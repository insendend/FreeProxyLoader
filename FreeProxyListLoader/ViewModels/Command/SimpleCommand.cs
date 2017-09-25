using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeProxyListLoader.ViewModels.Command
{
    class SimpleCommand : CommandBase
    {
        public SimpleCommand(Action execute, Func<bool> canExecute = null) : base(execute, canExecute)
        {
        }

        public override void Execute(object parameter)
        {
            execute();
        }
    }
}
