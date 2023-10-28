using Bro.Network;

namespace Bro.Client
{
    public class ClientApplication
    {
        public LocalContext LocalContext { get; private set; }
        public IClientContext GlobalContext { get; private set; }

        private readonly Timing.SystemUpdatesProvider _updatesProvider;

        public virtual Timing.IUpdatesProvider UpdatesProvider => _updatesProvider;

        protected ClientApplication()
        {
            _updatesProvider = new Timing.SystemUpdatesProvider();
            LocalContext = new LocalContext(this);
        }

        public virtual void Start(IClientContext globalContext, IClientContext entryContext)
        {
            NetworkOperationFactory.Initialize();
            
            GlobalContext = globalContext;
            
            GlobalContext.Load(this, () =>
            {
                LocalContext.SwitchTo(entryContext);    
            });
        }
    }
}