namespace MyToolkit.MachineLearning.WinRT.Feedforward.Training
{
    public interface ITrainable
    {
		FeedforwardNetwork Network { get; }
        double Error { get; }
        
		void Iteration();
    }
}
