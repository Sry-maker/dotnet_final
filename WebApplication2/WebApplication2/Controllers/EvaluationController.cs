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
    [ApiExplorerSettings(GroupName = "EvaluationController")]
    public class EvaluationController
    {
        /// <summary>用于各类之间接口的访问</summary>
        private static EvaluationController instance = new EvaluationController();
        /// <summary>对应类的实例</summary>
        public static EvaluationController Instance { get => instance; set => instance = value; }

        /// <summary>
        /// 按时间返回所有评价
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Dictionary<string,object>>), 200)]
        public ActionResult<List<Dictionary<string,object>>> getEvaluation()
        {
            List<Dictionary<string,object>> result = new List<Dictionary<string,object>>();
            EvaluationRepository evaluationRepo = new EvaluationRepository();
            foreach(var e in evaluationRepo.Evaluations.OrderByDescending(p => p.date).ToList())
            {
                Dictionary<string,object> value = new Dictionary<string,object>();
                value.Add("customer_id", e.customer_id);
                value.Add("evaluation_text", e.text);
                value.Add("evaluation_score", e.score);
                value.Add("evaluation_time", e.date.ToString());
                result.Add(value);
            }
            return result;
        }

        /// <summary>
        /// 添加评价
        /// </summary>
        /// <remarks>
        /// 添加成功返回1；连接失败返回-1；
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<int> addEvaluation(string customer_id,string evaluation_text,int evaluation_score)
        {
            EvaluationRepository evaluationRepo = new EvaluationRepository();
            Evaluation evaluation = new Evaluation();
            evaluation.score = evaluation_score;
            evaluation.date = DateTime.Now;
            evaluation.text = evaluation_text;
            evaluation.customer_id = customer_id;
            evaluationRepo.Evaluations.Add(evaluation);
            evaluationRepo.SaveChanges();
            return 1;
        }

        /// 删除评价
        /// </summary>
        /// <remarks>
        /// 删除成功返回1；连接失败返回-1；
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<int> deleteEvaluation(string customer_id, string evaluation_time)
        {
            EvaluationRepository evaluationRepo = new EvaluationRepository();
            DateTime time = DateTime.Parse(evaluation_time);
            var list = evaluationRepo.Evaluations.Where(p => p.customer_id.Equals(customer_id) &&
            p.date.Equals(time)).ToList();
            Evaluation evaluation = list.First();
            if(evaluation != null)
            {
                evaluationRepo.Evaluations.Remove(evaluation);
                evaluationRepo.SaveChanges();
            }
            return 1;
        }
    }
}
