using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3.Senior.Controls;

/// <summary>Ordering applied to rows in <see cref="PropertyGrid"/>.</summary>
public enum PropertyGridSortMode
{
    Categorized,
    Name,
    DeclaredOrder
}

public enum PropertyGridState
{
    Empty,
    Ready,
    Editing,
    Error
}

/// <summary>Optional host-owned conversion/editor hook. The provider never owns the selected object.</summary>
public interface IPropertyGridEditorProvider
{
    bool TryConvert(PropertyGridProperty property, object? proposedValue, out object? convertedValue, out string? error);
}

public sealed class PropertyGridGroup
{
    internal PropertyGridGroup(string name, IReadOnlyList<PropertyGridProperty> properties) { Name = name; Properties = properties; }
    public string Name { get; }
    public IReadOnlyList<PropertyGridProperty> Properties { get; }
}

/// <summary>Reflection metadata and current value for one editable property.</summary>
public sealed class PropertyGridProperty : INotifyPropertyChanged
{
    private object? _value;
    private string? _validationError;

    internal PropertyGridProperty(PropertyInfo property, object owner, int declarationOrder)
    {
        PropertyInfo = property;
        Owner = owner;
        DeclarationOrder = declarationOrder;
        Name = property.Name;
        DisplayName = property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? property.Name;
        Category = property.GetCustomAttribute<CategoryAttribute>()?.Category ?? "Miscellaneous";
        Description = property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
        PropertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        IsNullable = !property.PropertyType.IsValueType || Nullable.GetUnderlyingType(property.PropertyType) is not null;
        IsReadOnly = !property.CanWrite || property.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly == true;
        _value = property.GetValue(owner);
    }

    internal PropertyInfo PropertyInfo { get; }
    internal object Owner { get; }
    internal int DeclarationOrder { get; }

    public string Name { get; }
    public string DisplayName { get; }
    public string Category { get; }
    public string Description { get; }
    public Type PropertyType { get; }
    public bool IsNullable { get; }
    public bool IsReadOnly { get; }
    public bool IsExpanded { get; set; }
    public object? Value
    {
        get => _value;
        internal set
        {
            if (Equals(_value, value)) return;
            _value = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }

    public string? ValidationError
    {
        get => _validationError;
        internal set
        {
            if (string.Equals(_validationError, value, StringComparison.Ordinal)) return;
            _validationError = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValidationError)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public sealed class PropertyGridEditEventArgs : EventArgs
{
    public PropertyGridEditEventArgs(PropertyGridProperty property, object? oldValue, object? newValue)
    {
        Property = property;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public PropertyGridProperty Property { get; }
    public object? OldValue { get; }
    public object? NewValue { get; }
}

public sealed class PropertyGridEditFailedEventArgs : EventArgs
{
    public PropertyGridEditFailedEventArgs(PropertyGridProperty property, object? attemptedValue, string error)
    {
        Property = property;
        AttemptedValue = attemptedValue;
        Error = error;
    }

    public PropertyGridProperty Property { get; }
    public object? AttemptedValue { get; }
    public string Error { get; }
}

public sealed class PropertyGridHistoryChangedEventArgs(bool canUndo, bool canRedo) : EventArgs
{
    public bool CanUndo { get; } = canUndo;
    public bool CanRedo { get; } = canRedo;
}

/// <summary>
/// Reflection-backed editor surface. The control owns metadata and edit transactions;
/// an application remains the owner of the selected object's lifetime.
/// </summary>
public sealed class PropertyGrid : Control
{
    public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register(
        nameof(SelectedObject), typeof(object), typeof(PropertyGrid), new PropertyMetadata(null, OnSelectedObjectChanged));
    public static readonly DependencyProperty FilterTextProperty = DependencyProperty.Register(
        nameof(FilterText), typeof(string), typeof(PropertyGrid), new PropertyMetadata(string.Empty, OnFilterChanged));
    public static readonly DependencyProperty SortModeProperty = DependencyProperty.Register(
        nameof(SortMode), typeof(PropertyGridSortMode), typeof(PropertyGrid), new PropertyMetadata(PropertyGridSortMode.Categorized, OnFilterChanged));
    public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
        nameof(IsReadOnly), typeof(bool), typeof(PropertyGrid), new PropertyMetadata(false));

    private readonly ObservableCollection<PropertyGridProperty> _properties = new();
    private readonly ReadOnlyObservableCollection<PropertyGridProperty> _visibleProperties;
    private IReadOnlyList<PropertyGridGroup> _groups = [];
    private PropertyGridProperty? _editingProperty;
    private object? _editingOriginalValue;
    private readonly Stack<PropertyGridEditRecord> _undoStack = new();
    private readonly Stack<PropertyGridEditRecord> _redoStack = new();
    private bool _isReplayingHistory;

    public PropertyGrid()
    {
        _visibleProperties = new ReadOnlyObservableCollection<PropertyGridProperty>(_properties);
        DefaultStyleKey = typeof(PropertyGrid);
        Loaded += (_, _) => Refresh();
        IsTabStop = true;
        KeyDown += OnKeyDown;
    }

    public object? SelectedObject { get => GetValue(SelectedObjectProperty); set => SetValue(SelectedObjectProperty, value); }
    public string FilterText { get => (string?)GetValue(FilterTextProperty) ?? string.Empty; set => SetValue(FilterTextProperty, value); }
    public PropertyGridSortMode SortMode { get => (PropertyGridSortMode)GetValue(SortModeProperty); set => SetValue(SortModeProperty, value); }
    public bool IsReadOnly { get => (bool)GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }
    public IReadOnlyList<PropertyGridProperty> Properties => _visibleProperties;
    public IReadOnlyList<PropertyGridGroup> Groups => _groups;
    public PropertyGridState State { get; private set; } = PropertyGridState.Empty;
    public PropertyGridProperty? EditingProperty => _editingProperty;
    public IPropertyGridEditorProvider? EditorProvider { get; set; }
    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;

    public event EventHandler<PropertyGridEditEventArgs>? EditCommitted;
    public event EventHandler<PropertyGridEditFailedEventArgs>? EditFailed;
    public event EventHandler? PropertiesRefreshed;
    public event EventHandler<PropertyGridHistoryChangedEventArgs>? HistoryChanged;

    protected override AutomationPeer OnCreateAutomationPeer() => new PropertyGridAutomationPeer(this);

    public void Refresh()
    {
        _editingProperty = null;
        ClearHistory();
        _properties.Clear();
        if (SelectedObject is null)
        {
            _groups = [];
            State = PropertyGridState.Empty;
            PropertiesRefreshed?.Invoke(this, EventArgs.Empty);
            return;
        }

        var index = 0;
        foreach (var property in SelectedObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.GetIndexParameters().Length != 0 || !property.CanRead) continue;
            _properties.Add(new PropertyGridProperty(property, SelectedObject, index++));
        }

        ApplyOrderingAndFilter();
        State = PropertyGridState.Ready;
        PropertiesRefreshed?.Invoke(this, EventArgs.Empty);
    }

    public bool BeginEdit(string propertyName)
    {
        var property = _properties.FirstOrDefault(item => string.Equals(item.Name, propertyName, StringComparison.Ordinal));
        if (property is null || IsReadOnly || property.IsReadOnly) return false;
        _editingProperty = property;
        _editingOriginalValue = property.Value;
        property.ValidationError = null;
        State = PropertyGridState.Editing;
        return true;
    }

    public bool BeginEdit(PropertyGridProperty property)
    {
        ArgumentNullException.ThrowIfNull(property);
        return BeginEdit(property.Name);
    }

    public bool CommitEdit(object? value)
    {
        if (_editingProperty is null) return false;
        var property = _editingProperty;
        var oldValue = _editingOriginalValue;
        var result = TryCommit(property, value, out var error);
        if (!result)
        {
            property.ValidationError = error;
            State = PropertyGridState.Error;
            EditFailed?.Invoke(this, new PropertyGridEditFailedEventArgs(property, value, error));
            return false;
        }

        property.ValidationError = null;
        _editingProperty = null;
        State = PropertyGridState.Ready;
        EditCommitted?.Invoke(this, new PropertyGridEditEventArgs(property, oldValue, property.Value));
        RecordHistory(property, oldValue, property.Value);
        return true;
    }

    public bool CommitEdit(PropertyGridProperty property, object? value)
    {
        ArgumentNullException.ThrowIfNull(property);
        if (!ReferenceEquals(_editingProperty, property) && !BeginEdit(property)) return false;
        return CommitEdit(value);
    }

    public void CancelEdit()
    {
        if (_editingProperty is null) return;
        _editingProperty.ValidationError = null;
        _editingProperty = null;
        State = SelectedObject is null ? PropertyGridState.Empty : PropertyGridState.Ready;
    }

    public bool TrySetValue(string propertyName, object? value)
    {
        var property = _properties.FirstOrDefault(item => string.Equals(item.Name, propertyName, StringComparison.Ordinal));
        if (property is null || IsReadOnly || property.IsReadOnly) return false;
        var oldValue = property.Value;
        if (!TryCommit(property, value, out var error))
        {
            property.ValidationError = error;
            State = PropertyGridState.Error;
            EditFailed?.Invoke(this, new PropertyGridEditFailedEventArgs(property, value, error));
            return false;
        }

        property.ValidationError = null;
        State = PropertyGridState.Ready;
        EditCommitted?.Invoke(this, new PropertyGridEditEventArgs(property, oldValue, property.Value));
        RecordHistory(property, oldValue, property.Value);
        return true;
    }

    /// <summary>Reverts the most recent successful property edit on the current object.</summary>
    public bool Undo()
    {
        if (_undoStack.Count == 0) return false;
        var record = _undoStack.Pop();
        if (!TryApplyHistory(record.Property, record.OldValue))
        {
            _undoStack.Push(record);
            return false;
        }
        _redoStack.Push(record);
        HistoryChanged?.Invoke(this, new PropertyGridHistoryChangedEventArgs(CanUndo, CanRedo));
        return true;
    }

    /// <summary>Reapplies the most recently undone property edit.</summary>
    public bool Redo()
    {
        if (_redoStack.Count == 0) return false;
        var record = _redoStack.Pop();
        if (!TryApplyHistory(record.Property, record.NewValue))
        {
            _redoStack.Push(record);
            return false;
        }
        _undoStack.Push(record);
        HistoryChanged?.Invoke(this, new PropertyGridHistoryChangedEventArgs(CanUndo, CanRedo));
        return true;
    }

    public void ClearHistory()
    {
        if (_undoStack.Count == 0 && _redoStack.Count == 0) return;
        _undoStack.Clear();
        _redoStack.Clear();
        HistoryChanged?.Invoke(this, new PropertyGridHistoryChangedEventArgs(false, false));
    }

    private bool TryCommit(PropertyGridProperty descriptor, object? proposedValue, out string error)
    {
        error = string.Empty;
        if (SelectedObject is null || !ReferenceEquals(descriptor.Owner, SelectedObject))
        {
            error = "The selected object is no longer available.";
            return false;
        }
        if (IsReadOnly || descriptor.IsReadOnly)
        {
            error = "This property is read-only.";
            return false;
        }

        object? converted;
        try
        {
            if (EditorProvider is not null)
            {
                if (!EditorProvider.TryConvert(descriptor, proposedValue, out converted, out var providerError))
                {
                    error = providerError ?? "The editor rejected this value.";
                    return false;
                }
            }
            else converted = ConvertValue(proposedValue, descriptor.PropertyInfo.PropertyType);
            var context = new ValidationContext(SelectedObject!) { MemberName = descriptor.Name };
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateProperty(converted, context, results))
            {
                error = results.FirstOrDefault()?.ErrorMessage ?? "The value is not valid.";
                return false;
            }
        }
        catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException or ArgumentException)
        {
            error = ex.Message;
            return false;
        }
        catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
        {
            // A host editor/validation provider is an extension boundary; its
            // failure becomes an editable error state rather than escaping the UI thread.
            error = ex.Message;
            return false;
        }

        if (SelectedObject is IDataErrorInfo dataErrorInfo)
        {
            var validation = dataErrorInfo[descriptor.Name];
            if (!string.IsNullOrWhiteSpace(validation)) { error = validation; return false; }
        }

        try
        {
            descriptor.PropertyInfo.SetValue(descriptor.Owner, converted);
            descriptor.Value = converted;
            return true;
        }
        catch (Exception ex)
        {
            error = ex.InnerException?.Message ?? ex.Message;
            return false;
        }
    }

    private void ApplyOrderingAndFilter()
    {
        var text = FilterText.Trim();
        var filtered = _properties.Where(item => text.Length == 0 || item.DisplayName.Contains(text, StringComparison.CurrentCultureIgnoreCase) || item.Name.Contains(text, StringComparison.OrdinalIgnoreCase)).ToList();
        filtered = SortMode switch
        {
            PropertyGridSortMode.Name => filtered.OrderBy(item => item.DisplayName, StringComparer.CurrentCultureIgnoreCase).ToList(),
            PropertyGridSortMode.Categorized => filtered.OrderBy(item => item.Category, StringComparer.CurrentCultureIgnoreCase).ThenBy(item => item.DisplayName, StringComparer.CurrentCultureIgnoreCase).ToList(),
            _ => filtered.OrderBy(item => item.DeclarationOrder).ToList()
        };
        _properties.Clear();
        foreach (var property in filtered) _properties.Add(property);
        _groups = filtered.GroupBy(item => item.Category, StringComparer.CurrentCultureIgnoreCase)
            .Select(group => new PropertyGridGroup(group.Key, group.ToArray())).ToArray();
    }

    private void RecordHistory(PropertyGridProperty property, object? oldValue, object? newValue)
    {
        if (_isReplayingHistory || Equals(oldValue, newValue)) return;
        _undoStack.Push(new PropertyGridEditRecord(property, oldValue, newValue));
        _redoStack.Clear();
        HistoryChanged?.Invoke(this, new PropertyGridHistoryChangedEventArgs(CanUndo, CanRedo));
    }

    private bool TryApplyHistory(PropertyGridProperty property, object? value)
    {
        if (_isReplayingHistory || SelectedObject is null || !ReferenceEquals(property.Owner, SelectedObject)) return false;
        _isReplayingHistory = true;
        try
        {
            var oldValue = property.Value;
            if (!TryCommit(property, value, out var error))
            {
                property.ValidationError = error;
                State = PropertyGridState.Error;
                EditFailed?.Invoke(this, new PropertyGridEditFailedEventArgs(property, value, error));
                return false;
            }
            property.ValidationError = null;
            State = PropertyGridState.Ready;
            EditCommitted?.Invoke(this, new PropertyGridEditEventArgs(property, oldValue, property.Value));
            return true;
        }
        finally { _isReplayingHistory = false; }
    }

    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value is null)
        {
            if (targetType.IsValueType && Nullable.GetUnderlyingType(targetType) is null) throw new ArgumentNullException(nameof(value), "A value is required.");
            return null;
        }

        var nullableType = Nullable.GetUnderlyingType(targetType);
        var type = nullableType ?? targetType;
        if (type.IsInstanceOfType(value)) return value;
        if (type.IsEnum) return value is string text ? Enum.Parse(type, text, true) : Enum.ToObject(type, value);
        if (type == typeof(Guid)) return Guid.Parse(Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture)!);
        if (type == typeof(string)) return Convert.ToString(value, System.Globalization.CultureInfo.CurrentCulture);
        return Convert.ChangeType(value, type, System.Globalization.CultureInfo.CurrentCulture);
    }

    private static void OnSelectedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((PropertyGrid)d).Refresh();
    private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((PropertyGrid)d).Refresh();

    private void OnKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        var control = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Control);
        var shift = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
        if (control.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down) && e.Key == Windows.System.VirtualKey.Z)
        {
            if (shift.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down)) Redo(); else Undo();
            e.Handled = true;
        }
        else if (control.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down) && e.Key == Windows.System.VirtualKey.Y)
        {
            Redo();
            e.Handled = true;
        }
    }
}

internal sealed record PropertyGridEditRecord(PropertyGridProperty Property, object? OldValue, object? NewValue);

internal sealed class PropertyGridAutomationPeer : FrameworkElementAutomationPeer
{
    private readonly PropertyGrid _owner;
    public PropertyGridAutomationPeer(PropertyGrid owner) : base(owner) => _owner = owner;
    protected override string GetClassNameCore() => nameof(PropertyGrid);
    protected override string GetNameCore() => _owner.SelectedObject?.GetType().Name ?? nameof(PropertyGrid);
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.DataGrid;
}
