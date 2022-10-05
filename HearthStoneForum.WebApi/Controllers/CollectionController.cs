﻿using HearthStoneForum.IService;
using HearthStoneForum.Model;
using HearthStoneForum.Service;
using HearthStoneForum.WebApi.Utility.ApiResult;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HearthStoneForum.WebApi.Controllers
{
    [Route("api/collections")]
    [ApiController]
    public class CollectionController : ControllerBase
    {
        private readonly ICollectionService _iCollectionService;
        public CollectionController(ICollectionService iCollectionService)
        {
            _iCollectionService = iCollectionService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult>> GetCollection()
        {
            var data = await _iCollectionService.QueryAsync();
            if (data.Count == 0) return ApiResultHelper.Error("没有更多的值");
            return ApiResultHelper.Success(data);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult>> GetCollection(int id)
        {
            var collection = await _iCollectionService.FindAsync(id);
            if (collection == null) return ApiResultHelper.Error("没有更多的值");

            return ApiResultHelper.Success(collection);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResult>> Create(int invitationId)
        {
            int userId = Convert.ToInt32(this.User.FindFirst("UserId").Value);
            //此处为何用FindAsync而不用QueryAsync，因为FindAsync只会返回第一个，QueryAsync会查询所有，浪费时间和性能
            var data = await _iCollectionService.FindAsync(it => it.UserId == userId && it.InvitationId == invitationId);
            if (data != null) return ApiResultHelper.Error("添加失败");

            bool b = await _iCollectionService.CreateAsync(invitationId, userId);
            if (!b) return ApiResultHelper.Error("添加失败");

            var collection = await _iCollectionService.FindAsync(it => it.UserId == userId && it.InvitationId == invitationId);

            return ApiResultHelper.Success(collection);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            int userId = Convert.ToInt32(this.User.FindFirst("UserId").Value);
            var data = await _iCollectionService.FindAsync(it=>it.Id == id && it.UserId == userId);
            if (data == null) return ApiResultHelper.Error("没有找到该记录");

            bool b = await _iCollectionService.DeleteAsync(id);
            if (!b) return ApiResultHelper.Error("删除失败");
            return ApiResultHelper.Success(null);
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<ActionResult<ApiResult>> GetLikesByInvitationId(int invitationId)
        {
            int userId = Convert.ToInt32(this.User.FindFirst("UserId").Value);
            var data = await _iCollectionService.FindAsync(it => it.UserId == userId && it.InvitationId == invitationId);
            if (data == null) return ApiResultHelper.Error("没有找到该记录");

            return ApiResultHelper.Success(data);
        }
    }
}
