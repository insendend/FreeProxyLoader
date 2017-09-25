using System;
using System.Threading.Tasks;

namespace FreeProxyListLoader.ViewModels.Command
{
    class AsyncCommand : CommandBase
    {
        public AsyncCommand(Action execute, Func<bool> canExecute = null) : base(execute, canExecute)
        {
            
        }

        public override async void Execute(object parameter)
        {
            await Task.Run(execute);
        }
    }
}
