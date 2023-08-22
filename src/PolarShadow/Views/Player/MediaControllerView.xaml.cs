namespace PolarShadow.Views;

public partial class MediaControllerView : ContentView, IMediaController
{
	public MediaControllerView()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty VolumeProperty = BindableProperty.Create(nameof(Volume), typeof(int), typeof(MediaControllerView), 0);
    public int Volume
    {
        get => (int)GetValue(VolumeProperty);
        set => SetValue(VolumeProperty, value);
    }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(MediaControllerView), "");
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty TimeProperty = BindableProperty.Create(nameof(Time), typeof(TimeSpan), typeof(MediaControllerView), TimeSpan.FromSeconds(0));
    public TimeSpan Time
    {
        get => (TimeSpan)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    public void Pause()
    {
        throw new NotImplementedException();
    }

    public void Play()
    {
        throw new NotImplementedException();
    }

    private void PointerGestureRecognizer_PointerEntered(object sender, PointerEventArgs e)
    {
        Grid_Surface.IsVisible = true;
    }

    private void PointerGestureRecognizer_PointerExited(object sender, PointerEventArgs e)
    {
        Grid_Surface.IsVisible = false;
    }
}