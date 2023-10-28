using Bro.Client;
using Bro.Client.Network;
using Bro.Encryption;

namespace Bro.Network
{
    public class LetsEncryptTask : SubscribableTask<LetsEncryptTask>
    {
        private readonly DiffieHellmanProtocol _diffieHellmanProtocol = new DiffieHellmanProtocol(DiffieHellmanProtocol.EndpointSide.Client);
        private readonly NetworkEngine _engine;

        public LetsEncryptTask(NetworkEngine networkEngine)
        {
            _engine = networkEngine;
            Subscribe();
            
        }


        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);

            if (!_engine.IsConnected())
            {
                Bro.Log.Info( "Encryption cannot be established because there is no active connection to the remote server.");
                ProcessFail();
                return;
            }

            var operation = new LetsEncryptOperation(LetsEncryptOperation.EncryptionStep.PublicPart,
                _diffieHellmanProtocol.GenerateSelfPublicPart());
            _engine.Send(operation);
        }

        private void Subscribe()
        {
            _engine.OnLetsEncryptOperationReceived += OnLetsEncryptOperationReceived;
            _engine.OnStatusChanged += OnStatusChanged;
        }
        
        private void Unsubscribe()
        {
            _engine.OnLetsEncryptOperationReceived -= OnLetsEncryptOperationReceived;
            _engine.OnStatusChanged -= OnStatusChanged;
        }

        private void OnStatusChanged(NetworkStatus status, int code)
        {
            switch (status)
            {
                case NetworkStatus.Disconnected:
                    Bro.Log.Info("Encryption cannot be established because communication with the server was gone");
                    ProcessFail();
                    break;
            }
        }

        private void ProcessComplete()
        {
            Unsubscribe();
            Complete();
        }

        private void ProcessFail()
        {
            Unsubscribe();
            Fail();
        }

        private void OnLetsEncryptOperationReceived(LetsEncryptOperation operation)
        {
            var value = operation.Value;
            switch (operation.Step)
            {
                case LetsEncryptOperation.EncryptionStep.PublicPart:
                {
                    _diffieHellmanProtocol.SetForeignPublicPart(value);
                    var secretPart = _diffieHellmanProtocol.GenerateSelfSecterPart();
                    var response = new LetsEncryptOperation(LetsEncryptOperation.EncryptionStep.SecretPart, secretPart);
                    _engine.Send(response);
                }
                    break;
                case LetsEncryptOperation.EncryptionStep.SecretPart:
                {
                    _diffieHellmanProtocol.SetForeignSecretPart(value);

                    if (!_diffieHellmanProtocol.IsValid)
                    {
                        Bro.Log.Error(
                            "Encryption establishing failed after receiving secret part from server, discarding.");
                        ProcessFail();
                    }
                    else
                    {
                        var response = new LetsEncryptOperation(LetsEncryptOperation.EncryptionStep.HandShake, 0);
                        _engine.Send(response);
                        _engine.SetEncryption(_diffieHellmanProtocol.GetAESKey());
                    }
                }
                    break;
                case LetsEncryptOperation.EncryptionStep.HandShake:
                {
                    Bro.Log.Info("Encryption successfully established with remote server");
                    ProcessComplete();
                }
                    break;
            }
        }
    }
}