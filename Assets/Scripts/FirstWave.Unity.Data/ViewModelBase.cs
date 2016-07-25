namespace FirstWave.Unity.Data
{
	/// <summary>
	/// A simple extension on top of the NotifyableObject class, it will be able to listen to UnityEngine methods, like Update
	/// </summary>
	public abstract class ViewModelBase : NotifyableObject
	{
		public virtual void Update()
		{
		}
	}
}
