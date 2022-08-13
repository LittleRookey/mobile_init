using UnityEngine.Networking;

namespace CodeStage.AntiCheat.Utils
{
    internal class NonValidatingCertificateHandler : CertificateHandler
    {
        // using this to bypass possible ssl errors, i.e. due way too wrong time
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}