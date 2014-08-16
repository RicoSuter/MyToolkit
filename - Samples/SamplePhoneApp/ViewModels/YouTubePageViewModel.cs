namespace SamplePhoneApp.ViewModels
{
	public class YouTubePageViewModel : ViewModelBase
	{
	    private string _youTubeId;
        public string YouTubeId
	    {
            get { return _youTubeId; }
            set { Set(ref _youTubeId, value); }
	    }
	}
}
