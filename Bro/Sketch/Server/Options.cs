using System;
using System.IO;

namespace Bro.Sketch.Server
{
    public class Options
    {
        public readonly int Version;
        public readonly string SecretGate;
        public readonly string SecretKey;
        public readonly string ServerType;
        public readonly int ServerPort;
        public readonly string ServerEnvironment;

        public Options(string[] args)
        {
            var version = int.Parse(File.ReadAllText("version.txt"));
            var secretGate = string.Empty;
            var secretKey = string.Empty;
            var serverType = string.Empty;
            var serverEnvironment = string.Empty;
            var serverPort = 0;
            var showHelp = false;

            var options = new OptionSet()
            {
                {"secret_gate=", "{URL} to obtain secret data", v => secretGate = v},
                {"secret_key=", "{KEY} to decrypt the secret data", v => secretKey = v},
                {"server_type=", "{TYPE} of running server ( lobby or battle ).", v => serverType = v},
                {"server_environment=", "{ENVIRONMENT} of running server.", v => serverEnvironment = v},
                {"server_port=", "{PORT} of running server ( 3030 ).", v => serverPort = int.Parse(v)},
                {"h|help", "help", (v) => { showHelp = true; }}
            };

            options.Parse(args, true);

            if (showHelp)
            {
                options.WriteOptionDescriptions(Console.Out);
            }

            Version = version;
            SecretGate = secretGate;
            SecretKey = secretKey;
            ServerType = serverType;
            ServerPort = serverPort;
            ServerEnvironment = serverEnvironment;

            var secretKeyProvided = !string.IsNullOrEmpty(secretKey);

            Bro.Log.Info("application :: version = " + version);
            Bro.Log.Info("application :: secret gate = " + secretGate);
            Bro.Log.Info("application :: secret key = " + (secretKeyProvided ? " provided" : "not provided"));
            Bro.Log.Info("application :: server type = " + serverType);
            Bro.Log.Info("application :: server port = " + serverPort);
            Bro.Log.Info("application :: server environment = " + serverEnvironment);
        }
    }
}