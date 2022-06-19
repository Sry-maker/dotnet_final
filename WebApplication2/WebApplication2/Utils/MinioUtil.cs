using Minio;
using Minio.DataModel;
using Minio.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication2.Utils
{
    public class MinioUtil
    {
        public async static Task<bool> uploadPicture(string bucketname,string objectname,string filename)
        {
            Console.WriteLine("MinIO connecting");
            MinioClient minioClient = new MinioClient();
            minioClient = minioClient.WithEndpoint("116.62.208.68:9000").WithCredentials("JETHYFZIQ12OX3N1LJ3G", "zwEX3IWJ+dqhVLRM+AQOhpvQ5vBc+xooeXsOy7We").Build();
            Console.WriteLine("MinIO connected");
            bool flag = false;
            try
            {
                bool existsBucket = await minioClient.BucketExistsAsync(bucketname);
                if (existsBucket)
                {
                    Console.WriteLine("Bucket{0} exists",bucketname);
                    await minioClient.PutObjectAsync(bucketname, objectname, filename);
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶{0}不存在", bucketname));
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        public async static Task<bool> uploadPictureByStream(string bucketname, string objectname, Stream stream)
        {
            Console.WriteLine("MinIO connecting");
            MinioClient minioClient = new MinioClient();
            minioClient = minioClient.WithEndpoint("116.62.208.68:9000").WithCredentials("JETHYFZIQ12OX3N1LJ3G", "zwEX3IWJ+dqhVLRM+AQOhpvQ5vBc+xooeXsOy7We").Build();
            Console.WriteLine("MinIO connected");
            bool flag = false;
            try
            {
                bool existsBucket = await minioClient.BucketExistsAsync(bucketname);
                if (existsBucket)
                {
                    Console.WriteLine("Bucket{0} exists", bucketname);
                    await minioClient.PutObjectAsync(bucketname, objectname,stream,stream.Length);
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶{0}不存在", bucketname));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }
    }
}
