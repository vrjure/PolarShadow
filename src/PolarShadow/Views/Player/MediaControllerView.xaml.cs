using LibVLCSharp.Shared;
using System.Windows.Input;

namespace PolarShadow.Views;

public partial class MediaControllerView : ContentView, IMediaController
{
	public MediaControllerView()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty MediaPlayerProperty = BindableProperty.Create(nameof(MediaPlayer), typeof(MediaPlayer), typeof(MediaControllerView), default);
    public MediaPlayer MediaPlayer
    {
        get => (MediaPlayer)GetValue(MediaPlayerProperty);
        set => SetValue(MediaPlayerProperty, value);
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

    public static readonly BindableProperty LengthProperty = BindableProperty.Create(nameof(Length), typeof(TimeSpan), typeof(MediaControllerView), TimeSpan.Zero);
    public TimeSpan Length
    {
        get => (TimeSpan)GetValue(LengthProperty);
        set => SetValue(LengthProperty, value);
    }

    public static readonly BindableProperty TimeProperty = BindableProperty.Create(nameof(Time), typeof(TimeSpan), typeof(MediaControllerView), TimeSpan.Zero);
    public TimeSpan Time
    {
        get => (TimeSpan)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    public static readonly BindableProperty IsPlayingProperty = BindableProperty.Create(nameof(IsPlaying), typeof(bool), typeof(MediaControllerView), false);
    public bool IsPlaying
    {
        get => (bool)GetValue(IsPlayingProperty);
        set => SetValue(IsPlayingProperty, value);
    }

    private ICommand _playCommand;
    public ICommand PlayCommand => _playCommand ??= new Command(Play);

    private ICommand _pauseCommand;
    public ICommand PauseCommand => _pauseCommand ??= new Command(Pause);

    private ICommand _pointerEnterCommand;
    public ICommand PointerEnterCommand => _pointerEnterCommand ??= new Command<View>(PointerEnter);

    private ICommand _pointerExitedCommand;
    public ICommand PointerExitedCommand => _pointerExitedCommand ??= new Command<View>(PointerExited);

    public void Pause()
    {
        if (MediaPlayer == null) return;
        MediaPlayer.Pause();
        IsPlaying = MediaPlayer.IsPlaying;
    }

    public void Play()
    {
        if (MediaPlayer == null) return;
        MediaPlayer.Play();
        IsPlaying = MediaPlayer.IsPlaying;
    }

    private void PointerEnter(View view)
    {
        view?.FadeTo(1, 500);
    }

    private void PointerExited(View view)
    {
        view?.FadeTo(0, 500);
    }

    private void PointerGestureRecognizer_PointerEntered(object sender, PointerEventArgs e)
    {
        PointerEnter(sender as View);
    }

    private void PointerGestureRecognizer_PointerExited(object sender, PointerEventArgs e)
    {
        PointerExited(sender as View);
    }
}