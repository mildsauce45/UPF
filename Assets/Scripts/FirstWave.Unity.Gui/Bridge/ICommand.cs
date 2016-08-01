namespace FirstWave.Unity.Gui.Bridge
{
    public interface ICommand
    {
        bool CanExecute(object context);

        void Execute(object context);
    }
}
