using System;

namespace NailSalonTycoon.GameLevel.Clients
{
    [Serializable]
    public struct ClientConfig
    {
        public ClientView view;
        public ClientType type;
    }
}
