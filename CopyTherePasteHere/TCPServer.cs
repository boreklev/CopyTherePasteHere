using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CopyTherePasteHere
{
    public class TcpServer
    {
        #region Public.     
        // Create new instance of TcpServer.
        public TcpServer()
        {
            tcpListener = new TcpListener(IPAddress.Any, App.SERVER_PORT);
        }

        // Starts receiving incoming requests.      
        public void Start()
        {
            tcpListener.Start();
            cancelToken = cancelTokenSource.Token;
            tcpListener.BeginAcceptTcpClient(ProcessRequest, tcpListener);
        }

        // Stops receiving incoming requests.
        public void Stop()
        {
            // If listening has been cancelled, simply go out from method.
            if (cancelToken.IsCancellationRequested)
            {
                return;
            }

            // Cancels listening.
            cancelTokenSource.Cancel();

            // Waits a little, to guarantee 
            // that all operation receive information about cancellation.
            Thread.Sleep(100);
            tcpListener.Stop();
        }
        #endregion

        #region Private.
        // Process single request.
        private void ProcessRequest(IAsyncResult ar)
        {
            //Stop if operation was cancelled.
            if (cancelToken.IsCancellationRequested)
            {
                return;
            }

            var listener = ar.AsyncState as TcpListener;
            if (listener == null)
            {
                return;
            }

            // Check cancellation again. Stop if operation was cancelled.
            if (cancelToken.IsCancellationRequested)
            {
                return;
            }

            // Starts waiting for the next request.
            listener.BeginAcceptTcpClient(ProcessRequest, listener);

            // Gets client and starts processing received request.
            using (TcpClient client = listener.EndAcceptTcpClient(ar))
            {
                var rp = new RequestProcessor();
                rp.Proccess(client);
            }
        }
        #endregion

        #region Fields.
        private CancellationToken cancelToken;
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        private TcpListener tcpListener;
        #endregion
    }
}
