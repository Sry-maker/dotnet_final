using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using WebApplication2.Repository;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    /// <summary>
    /// 设置路由
    /// </summary>
    [Route("api/[Controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "DishController")]
    public class DishController
    {
        /// <summary>用于各类之间接口的访问</summary>
        private static DishController instance = new DishController();
        /// <summary>对应类的实例</summary>
        public static DishController Instance { get => instance; set => instance = value; }

        public static byte[] Url_To_Byte(string filePath)
        {
            //第一步：读取图片到byte数组
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(filePath);

            byte[] bytes;
            using (Stream stream = request.GetResponse().GetResponseStream())
            {
                using (MemoryStream mstream = new MemoryStream())
                {
                    int count = 0;
                    byte[] buffer = new byte[1024];
                    int readNum = 0;
                    while ((readNum = stream.Read(buffer, 0, 1024)) > 0)
                    {
                        count = count + readNum;
                        mstream.Write(buffer, 0, readNum);
                    }
                    mstream.Position = 0;
                    using (BinaryReader br = new BinaryReader(mstream))
                    {
                        bytes = br.ReadBytes(count);
                    }
                }
            }
            return bytes;


        }

        /// <summary>
        /// 获取菜品类型
        /// </summary>
        /// <param name="dish_id"></param>
        /// <returns></returns>
        [HttpGet]
        public List<string> getDishType(string dish_id)
        {
            var typeRepo = new TypeRepository();
            return typeRepo.Types.Where(p => p.dish_id.Equals(dish_id)).Select(p => p.dish_type).ToList();
        }

        /// <summary>
        /// 返回菜单
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(List<Dish>), 200)]
        [HttpGet]
        public List<Dictionary<string,object>> getAllDish()
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            var dishRepo = new DishRepository();
            List<Dish> dishes = dishRepo.Dishes.ToList();
            foreach(var dish in dishes)
            {
                Dictionary<string, object> element = new Dictionary<string, object>();
                element.Add("dish_id", dish.dish_id);
                element.Add("dish_name", dish.name);
                element.Add("dish_price", dish.price);
                element.Add("dish_type", getDishType(dish.dish_id));
                element.Add("dish_rate", dish.count);
                element.Add("dish_info", dish.info);
                element.Add("index", dish.index);
                result.Add(element);
            }
            return result;
        }

        /// <summary>
        /// 返回某道菜品
        /// </summary>
        /// <param name="dish_id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Dish), 200)]
        [HttpGet("{dish_id}")]
        public Dish getDish(String dish_id)
        {
            using (var dishRepo = new DishRepository())
            {
                Dish dish = dishRepo.Dishes.Find(dish_id);
                return dish;
            }
        }

        /// <summary>
        /// 返回菜品图片
        /// </summary>
        /// <param name="dish_id"></param>
        /// <returns></returns>
        /// <remarks>
        /// 返回byte[]，异常返回：null
        /// </remarks>
        [HttpGet]
        public ActionResult<byte[]> getDishPict(string dish_id)
        {
            DishRepository dishRepo = new DishRepository();
            if (dishRepo.Dishes.Find(dish_id) != null)
            {
                string? url = dishRepo.Dishes.Find(dish_id).url;
                if (url != null)
                {
                    return Url_To_Byte(url);
                }
            }
            return null;
        }

        /// <summary>
        /// 返回所有菜品图片
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<byte[]>> getAllDishPict()
        {
            List<byte[]> result = new List<byte[]>();
            var dishRepo = new DishRepository();
            foreach(var dish_id in dishRepo.Dishes.Select(p => p.dish_id).ToList())
            {
                result.Add(getDishPict(dish_id).Value);
            }
            return result;
        }


        /// <summary>
        /// 返回Top4菜品
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Dictionary<string,object>> getTop4()
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            var dishRepo = new DishRepository();
            var topList = dishRepo.Dishes.Where(p => !p.dish_id.StartsWith("c")).OrderByDescending(p => p.count).Select(p => p.dish_id).ToList();
            int i = 0;
            foreach(var dish_id in topList)
            {
                if (i >= 4) break;
                Dictionary<string, object> element = new Dictionary<string, object>();
                Dish dish = dishRepo.Dishes.Find(dish_id);
                i++;
                element.Add("dish_id", dish_id);
                element.Add("dish_name", dish.name);
                element.Add("dish_price", dish.price);
                element.Add("dish_pict", getDishPict(dish_id));
                element.Add("dish_rate", dish.count);
                element.Add("dish_info", dish.info);
                element.Add("index",dish.index);
                result.Add(element);
            }
            return result;
        }
    }
}
