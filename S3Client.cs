using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace S3Test
{

    public class S3Client
    {
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _endpoint;


        readonly AmazonS3Client _client;

        public S3Client(string accessKey, string secretKey, string endpoint)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => true;

            _accessKey = accessKey;
            _secretKey = secretKey;
            _endpoint = endpoint;

            var md5 = System.Security.Cryptography.MD5.Create();
            //var auth = new BasicAWSCredentials(_accessKey, _secretKey),
            //b64Uname = Convert.ToBase64String(Encoding.UTF8.GetBytes(_accessKey));
            //md5Pword = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(_secretKey))).Replace("-", String.Empty).ToLower();
            var config = new AmazonS3Config { ServiceURL = _endpoint, UseHttp = true };
            _client = new AmazonS3Client(_accessKey, _secretKey, config);
        }


        public List<S3Bucket> ListBuckets()
        {
            ListBucketsResponse buckets = _client.ListBuckets();
            return buckets.Buckets;
        }
        public string Get(string tableName, string id)
        {

            var req = new GetObjectRequest { BucketName = tableName, Key = id };
            using (var stream = _client.GetObject(req).ResponseStream)
            {
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }


        public void Put(string tableName, string id, string fileContent, params Tuple<string, string>[] metadatas)
        {
            var req = new PutObjectRequest();
            if (metadatas != null)
            {
                foreach (var (key, value) in metadatas)
                {
                    req.Metadata[key] = value;
                }
            }
            req.BucketName = tableName;
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(fileContent);
                req.InputStream = stream;
                req.Key = id;
                _client.PutObject(req);
            }
        }
    }
}
