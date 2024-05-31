using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        // Create two peers
        Peer alice = new Peer("Alice");
        Peer bob = new Peer("Bob");

        // Exchange public keys
        string alicePublicKey = alice.GetPublicKey();
        string bobPublicKey = bob.GetPublicKey();

        // Display public keys
        Console.WriteLine("Alice's Public Key: " + alicePublicKey);
        Console.WriteLine("Bob's Public Key: " + bobPublicKey);

        // Alice sends an encrypted message to Bob
        string originalMessage = "Hello Bob, this is Alice!";
        string encryptedMessage = alice.Encrypt(originalMessage, bobPublicKey);
        Console.WriteLine("Encrypted Message from Alice to Bob: " + encryptedMessage);

        // Bob decrypts the message
        string decryptedMessage = bob.Decrypt(encryptedMessage);
        Console.WriteLine("Decrypted Message at Bob's end: " + decryptedMessage);
    }
}

public class Peer
{
    public string Name { get; private set; }
    private RSA rsa;

    public Peer(string name)
    {
        Name = name;
        rsa = RSA.Create();
        rsa.KeySize = 2048;
    }

    public string GetPublicKey()
    {
        return Convert.ToBase64String(rsa.ExportRSAPublicKey());
    }

    public string Encrypt(string data, string publicKey)
    {
        using (RSA rsaEncrypt = RSA.Create())
        {
            rsaEncrypt.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(data);
            byte[] encryptedData = rsaEncrypt.Encrypt(dataToEncrypt, RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(encryptedData);
        }
    }

    //can only decrypt messages encrypted with his own public key
    public string Decrypt(string encryptedData)
    {
        byte[] dataToDecrypt = Convert.FromBase64String(encryptedData);
        byte[] decryptedData = rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.OaepSHA256);
        return Encoding.UTF8.GetString(decryptedData);
    }
}
