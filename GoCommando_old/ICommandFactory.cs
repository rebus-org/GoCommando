using System;

namespace GoCommando
{
    /// <summary>
    /// Can be implemented to supply a custom command factory that will be given a chance to create command instances
    /// and dispose of them after use
    /// </summary>
    public interface ICommandFactory
    {
        /// <summary>
        /// Should create a new command instance of the given <paramref name="commandType"/>
        /// </summary>
        ICommand Create(Type commandType);

        /// <summary>
        /// Should release the command instance - probably by disposing it or delegating the disposal to an IoC container
        /// </summary>
        void Release(ICommand command);
    }
}