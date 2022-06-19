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
    // <summary>
    /// 设置路由
    /// </summary>
    [Route("api/[Controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "reservation")]

    public class ReservationController
    {
        public static string generateId()
        {
            string reservation_id = "";
            int yr = DateTime.Now.Year;
            int mon = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            string date = yr.ToString().Substring(2, 2) + mon.ToString() + day.ToString();
            if (date.Length == 4)
            {
                date = date.Insert(2, "0");
                date = date.Insert(4, "0");
            }
            else if (date.Length == 5 && mon < 10)
            {
                date = date.Insert(2, "0");
            }
            else if (date.Length == 5 && day < 10)
            {
                date = date.Insert(4, "0");
            }
            reservation_id = date + "0000";
            using (var reservationRepo = new ReservationRepository())
            {
                string? max_id = reservationRepo.Reservations.
                    Where(p => p.reservation_id.StartsWith(date)).
                    Select(p => p.reservation_id).
                    Max();
                if (max_id != null)
                {
                    reservation_id = (long.Parse(max_id) + 1).ToString();
                }
            }
            return reservation_id;
        }

        /// <summary>
        /// 获取所有可预约餐桌
        /// </summary>
        /// <param name="customer_id"></param>
        /// <param name="reservation_date">yyyy-mm-dd</param>
        ///<param name="start_time">hh24:mi</param>
        ///<param name="num"></param>
        /// <returns></returns>
        [HttpGet]
        public List<DiningTable> getAllUnreservedTable(string reservation_date, string start_time, int num)
        {
            var reservationRepo = new ReservationRepository();

            var usedList = reservationRepo.Reservations.
                        Where(p => p.reservation_date.Equals(DateTime.Parse(reservation_date)) && p.start_time.Equals(start_time) && p.state==0)
                        .Select(p=>p.table_id).ToList();

            return reservationRepo.Tables.Where(p => p.table_id.StartsWith("r") && p.capacity >= num)
                .Where(p => !usedList.Contains(p.table_id))
                .OrderBy(p => p.table_id)
                .ToList();

        }

        /// <summary>
        /// 预约餐桌
        /// </summary>
        /// <param name="customer_id"></param>
        /// <param name="reservation_date">yyyy-mm-dd</param>
        ///<param name="start_time">hh24:mi</param>
        ///<param name="end_time"></param>
        ///<param name="table_id"></param>
        /// <returns></returns>
        /// <remarks>
        /// 传入：日期，顾客id，餐桌id，开始时间，结束时间
        /// 返回：1-预约成功
        /// 2-预约已满
        /// 3-当日已经预约
        /// 4-用户不存在
        /// </remarks>
        [HttpPost]
        public int reserveTable(string customer_id,string reservation_date,string start_time,string end_time,string table_id)
        {
            using(var customerRepo = new CustomerRepository())
            {
                if (customerRepo.Customers.Find(customer_id) == null) return 4;
            }
            var reservationRepo = new ReservationRepository();
            if (reservationRepo.Reservations.Where(p => p.reservation_date.Equals(DateTime.Parse(reservation_date))
                && p.start_time.Equals(start_time) && p.customer_id.Equals(customer_id))
                .Count() > 0) return 3;
            if (getAllUnreservedTable(reservation_date, start_time, 0).Select(p => p.table_id).Contains(table_id))
            {
                Reservation reservation = new Reservation();
                reservation.start_time = start_time;
                reservation.end_time = end_time;
                reservation.table_id = table_id;
                reservation.reservation_date = DateTime.Parse(reservation_date);
                reservation.state = 0;
                reservation.customer_id = customer_id;
                reservation.reservation_id = generateId();
                reservationRepo.Add(reservation);
                reservationRepo.SaveChanges();
                return 1;
            }
            return 2 ;
        }

        /// <summary>
        /// 取消预约
        /// </summary>
        /// <param name="reservation_id"></param>
        /// <returns></returns>
        /// <remarks>
        /// 需传入reservation_id  返回：0-失败 1-删除成功
        /// </remarks>
        [HttpPost]
        public ActionResult<int> cancelReservation(string reservation_id)
        {
            ReservationRepository reservationRepo = new ReservationRepository();
            var reservation = reservationRepo.Reservations.Find(reservation_id);
            if (reservation != null && reservation.state == 0 )
            {
                if(reservation.reservation_date > DateTime.Now)
                {
                    reservation.state = 2;
                    reservationRepo.SaveChanges();
                    return 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// 按照预约状态筛选出某位顾客预定信息
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 第一位传入customer_id，后面传state，按照所传的state参数顺序返回预定信息
        /// </remarks>
        [HttpPost]
        public List<Dictionary<string,object>> getOnesReservationByState([FromBody] List<string> param)
        {
            Dictionary<string, int> map = new Dictionary<string, int> { { "等待", 0 }, { "完成", 1 }, { "取消", 2 }, { "过期", 3 } };
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            string customer_id = param[0];
            var reservationRepo = new ReservationRepository();
            for(int i = 1; i < param.Count(); i++)
            {
                var list = reservationRepo.Reservations.Where(p => p.customer_id.Equals(customer_id) && p.state == map[param[i]])
                    .OrderByDescending(p => p.reservation_date).ToList();
                foreach(var r in list)
                {
                    Dictionary<string, object> element = new Dictionary<string, object>();
                    element.Add("reservation_id", r.reservation_id);
                    element.Add("start_time", r.start_time);
                    element.Add("end_time", r.end_time);
                    element.Add("table_id", r.table_id);
                    element.Add("reservation_date", r.reservation_date.ToString().Replace(" 0:00:00",""));
                    element.Add("customer_id", r.customer_id);
                    element.Add("customer_name", CustomerController.getCustomer(r.customer_id) == null ? "" : CustomerController.getCustomer(r.customer_id).customer_name);
                    element.Add("reservation_state", param[i]);
                    result.Add(element);
                }
            }
            if (param.Count() == 1)
            {
                return getReservationInformation(customer_id);
            }
            return result;
        }

        /// <summary>
        /// 查询顾客所有预定信息(供顾客端)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(List<Dictionary<string, object>>), 200)]
        [HttpGet]
        public List<Dictionary<string, object>> getReservationInformation(string id)
        {
            Dictionary<string, int> map = new Dictionary<string, int> { { "等待", 0 }, { "完成", 1 }, { "取消", 2 }, { "过期", 3 } };
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            string customer_id = id;
            var reservationRepo = new ReservationRepository();
            var list = reservationRepo.Reservations.Where(p => p.customer_id.Equals(customer_id))
                    .OrderByDescending(p => p.reservation_date).ToList();
            foreach (var r in list)
            {
                Dictionary<string, object> element = new Dictionary<string, object>();
                element.Add("reservation_id", r.reservation_id);
                element.Add("start_time", r.start_time);
                element.Add("end_time", r.end_time);
                element.Add("table_id", r.table_id);
                element.Add("reservation_date", r.reservation_date.ToString().Replace(" 0:00:00", ""));
                element.Add("customer_id", r.customer_id);
                element.Add("customer_name", CustomerController.getCustomer(r.customer_id) == null ? "" : CustomerController.getCustomer(r.customer_id).customer_name);
                element.Add("reservation_state", map.Keys.Where(p => map[p].Equals(r.state)).First());
                result.Add(element);
            }
            return result;
        }
    }
}
