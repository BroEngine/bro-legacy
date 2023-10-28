using Bro.Encryption;
using Bro.Network;
using Bro.Server.Network;

namespace Bro.Server.Observers
{
    public class LetsEncryptObserver
    {
        private readonly DiffieHellmanProtocol _diffieHellmanProtocol = new DiffieHellmanProtocol(DiffieHellmanProtocol.EndpointSide.Server);

        public void OnReceive(LetsEncryptOperation operation, IClientPeer peer)
        {
            var value = operation.Value;
            switch (operation.Step)
            {
                case LetsEncryptOperation.EncryptionStep.PublicPart:
                {
                    var publicPart = _diffieHellmanProtocol.GenerateSelfPublicPart();
                    _diffieHellmanProtocol.SetForeignPublicPart(value);
                    var response = new LetsEncryptOperation(LetsEncryptOperation.EncryptionStep.PublicPart, publicPart);
                    peer.Send(response);
                }
                    break;
                case LetsEncryptOperation.EncryptionStep.SecretPart:
                {
                    var secretPart = _diffieHellmanProtocol.GenerateSelfSecterPart();
                    _diffieHellmanProtocol.SetForeignSecretPart(value);

                    if (!_diffieHellmanProtocol.IsValid)
                    {
                        Bro.Log.Error("For peerId = " + peer.PeerId +  "  encryption establishing failed after receiving secret part, discarding.");
                        Discard(peer);
                    }
                    else
                    {
                        var response = new LetsEncryptOperation(LetsEncryptOperation.EncryptionStep.SecretPart, secretPart);
                        peer.Send(response);
                    }
                }
                    break;
                case LetsEncryptOperation.EncryptionStep.HandShake:
                {
                    if (!_diffieHellmanProtocol.IsValid)
                    {
                        Bro.Log.Error("For peerId = " + peer.PeerId + "  encryption establishing failed after receiving handshake part, discarding.");
                        Discard(peer);
                    }
                    else
                    {
                        var response = new LetsEncryptOperation(LetsEncryptOperation.EncryptionStep.HandShake, value);
                        peer.SetEncryption(_diffieHellmanProtocol.GetAESKey());
                        peer.Send(response);
                    }
                }
                    break;
            }
        }

        private void Discard(IClientPeer peer)
        {
            _diffieHellmanProtocol.Reset();
            var response = new LetsEncryptOperation(LetsEncryptOperation.EncryptionStep.Discard, 0);
        }
    }
}