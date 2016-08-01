using System;

namespace FirstWave.Unity.Gui.Bridge
{
    public class ExecutableCommand : ICommand
    {
        private Action<object> function;
        private Func<object, bool> canExecute;

        public ExecutableCommand(Action<object> function)
        {
            this.function = function;
            canExecute = o => true;
        }

        public ExecutableCommand(Action<object> function, Func<object, bool> canExecute)
        {
            this.function = function;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object context)
        {
            if (canExecute == null)
                return true;

            return canExecute(context);
        }

        public void Execute(object context)
        {
            function(context);
        }
    }
}
