namespace PolarShadow.Views;

public partial class VLCPLayer : ContentView
{
	public VLCPLayer()
	{
		InitializeComponent();
	}

    public readonly static BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(Uri), typeof(VLCPLayer), propertyChanged: OnSourcePropertyChanged);
    public Uri Source
    {
        get => (Uri)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    private static void OnSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var player = bindable as VLCPLayer;
    }
}