namespace FirstWave.Unity.Core.Utilities
{
	public interface ITimer
	{
		bool IsComplete { get; }
		bool Enabled { get; }

		void Start();
		void Update();
		void Stop();
	}
}
