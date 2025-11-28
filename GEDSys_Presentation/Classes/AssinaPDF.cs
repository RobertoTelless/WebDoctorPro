using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
// NECESSÁRIO!
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System.Linq; // Essencial para o ToArray() ou casting

public class AssinaturaDigital
{
    public static void AssinarPdfComPfx(string srcFile, string destFile, string pfxPath, string pfxPassword)
    {
        X509Certificate2 cert;
        try
        {
            // 1. CARREGAR O CERTIFICADO PFX
            cert = new X509Certificate2(pfxPath, pfxPassword, X509KeyStorageFlags.Exportable);
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao carregar o certificado PFX. Verifique o caminho e a senha.", ex);
        }

        // --- CORREÇÃO DA CHAVE PRIVADA E DA CADEIA ---

        // CORREÇÃO 1: Garante que o tipo da chave privada é AsymmetricKeyParameter (base)
        AsymmetricKeyParameter pk = null;

        // Tenta extrair a chave privada.
        // Se 'cert' não for reconhecido aqui, verifique a declaração e o escopo da variável 'cert'.
        if (cert.HasPrivateKey)
        {
            // O tipo de retorno de GetKeyPair é AsymmetricCipherKeyPair.
            // A sintaxe .Private é a correta, mas pode falhar se a chave não for exportável.
            pk = DotNetUtilities.GetKeyPair(cert).Private;
        }
        else
        {
            throw new Exception("O certificado não possui chave privada ou ela não pôde ser extraída.");
        }

        // 2. OBTÉM A CADEIA DE CERTIFICADOS COMPATÍVEL COM ITEXTSHARP

        // CORREÇÃO 2: Usa o método .GetCertificateChain() e converte para array
        // IX509Certificate é a interface do iTextSharp. O compilador deveria encontrá-la.
        iTextSharp.text.pdf.security.IX509Certificate[] chain;

        // Instancia o objeto de assinatura, que fornece a cadeia no formato iTextSharp.
        X509Certificate2Signature signature = new X509Certificate2Signature(cert, DigestAlgorithms.SHA256);

        // Pega a cadeia. Se GetCertificateChain() não for reconhecido, a versão do iTextSharp está errada, 
        // ou há um conflito de namespaces.
        chain = signature.GetCertificateChain();

        // **Alternativa Rápida para forçar o reconhecimento se o erro persistir:**
        // chain = new iTextSharp.text.pdf.security.IX509Certificate[] { signature.GetCertificateChain().First() };


        // 3. CONFIGURAR O OBJETO PDF SIGNER
        using (FileStream fout = new FileStream(destFile, FileMode.Create))
        {
            PdfReader reader = new PdfReader(srcFile);
            PdfStamper stp = PdfStamper.CreateSignature(reader, fout, '\0');
            PdfSignatureAppearance sap = stp.SignatureAppearance;

            sap.Reason = "Aceite do Contrato Eletrônico";
            sap.Location = "Rio de Janeiro, BR";
            sap.SignDate = DateTime.Now;
            sap.SetVisibleSignature(new iTextSharp.text.Rectangle(100, 100, 300, 200), 1, "SignatureField");

            // 4. CRIAR A ASSINATURA DIGITAL
            // CORREÇÃO 3: Remove a linha duplicada e usa a instância única.
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);

            // 5. Assina o documento
            // CORREÇÃO 4: A chamada a MakeSignature agora deve funcionar, pois 'chain' tem o tipo correto.
            MakeSignature.SignDetached(sap, pks, chain, null, null, null, 0, CryptoStandard.CMS);

            stp.Close();
        }
    }
}