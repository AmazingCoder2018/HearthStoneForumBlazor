﻿using HearthStoneForum.IService;
using HearthStoneForum.Model;
using HearthStoneForum.Model.DTOView;
using HearthStoneForum.WebApi.Utility.ApiResult;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SqlSugar;
using System.Drawing;
using System.Security.Policy;
using System.Xml.Linq;

namespace HearthStoneForum.WebApi.Controllers
{
    [Route("api/invitations")]
    [ApiController]
    public class InvitationController : ControllerBase
    {
        private readonly IInvitationService _iInvitationService;
        public InvitationController(IInvitationService iInvitationService)
        {
            _iInvitationService = iInvitationService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult>> GetInvitation()
        {
            var data = await _iInvitationService.QueryDTOAsync<InvitationDTOView>();
            if (data.Count == 0) return ApiResultHelper.Error("没有更多的值");
            return ApiResultHelper.Success(data);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult>> GetInvitation(int id)
        {
            var invitation = (await _iInvitationService.QueryDTOAsync<InvitationDTOView>(it => it.Id == id)).FirstOrDefault();
            if (invitation == null) return ApiResultHelper.Error("没有更多的值");

            return ApiResultHelper.Success(invitation);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult>> Create(Invitation invitation)
        {
            bool b = await _iInvitationService.CreateAsync(invitation);
            if (!b) return ApiResultHelper.Error("添加失败");

            return ApiResultHelper.Success(invitation);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            bool b = await _iInvitationService.DeleteAsync(id);
            if (!b) return ApiResultHelper.Error("删除失败");
            return ApiResultHelper.Success(null);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult>> Edit(int id, Invitation invitation)
        {
            var oldInvitation = await _iInvitationService.FindAsync(id);
            if (oldInvitation == null) return ApiResultHelper.Error("没有找到该记录");

            bool b = await _iInvitationService.EditAsync(invitation);
            if (!b) return ApiResultHelper.Error("修改失败");
            return ApiResultHelper.Success(invitation);
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResult>> GetInvitationByName(string name)
        {
            var data = await _iInvitationService.QueryDTOAsync<InvitationDTOView>(it => it.Title.ToLower().Contains(name.ToLower()));
            if (data.Count == 0) return ApiResultHelper.Error("未找到想要搜索的数据");
            return ApiResultHelper.Success(data);
        }

        [HttpGet("new")]
        public async Task<ActionResult<ApiResult>> GetNewInvitation()
        {
            var data = await _iInvitationService.GetNewInvitations();
            if (data.Count == 0) return ApiResultHelper.Error("没有更多的值");
            return ApiResultHelper.Success(data);
        }

        [HttpGet("recommend")]
        public async Task<ActionResult<ApiResult>> GetRecommendInvitation()
        {
            var data = await _iInvitationService.GetRecommendInvitations();
            if (data.Count == 0) return ApiResultHelper.Error("没有更多的值");
            return ApiResultHelper.Success(data);
        }

        [HttpGet("like")]
        public async Task<ActionResult<ApiResult>> GetLikeInvitation(int id, int page, int size)
        {
            RefAsync<int> total = 0;
            var data = await _iInvitationService.GetLikeInvitations(it => it.UserId == id, page, size, total);
            if (data.Count == 0) return ApiResultHelper.Error("没有更多的值");
            return ApiResultHelper.Success(data, total);
        }

        [HttpGet("collection")]
        public async Task<ActionResult<ApiResult>> GetCollectionInvitation(int id, int page, int size)
        {
            RefAsync<int> total = 0;
            var data = await _iInvitationService.GetCollectionInvitations(it => it.UserId == id, page, size, total);
            if (data.Count == 0) return ApiResultHelper.Error("没有更多的值");
            return ApiResultHelper.Success(data, total);
        }

        [HttpGet("viewRecord")]
        public async Task<ActionResult<ApiResult>> GetViewRecordInvitation(int id, int page, int size)
        {
            RefAsync<int> total = 0;
            var data = await _iInvitationService.GetViewRecordInvitations(it => it.UserId == id, page, size, total);
            if (data.Count == 0) return ApiResultHelper.Error("没有更多的值");
            return ApiResultHelper.Success(data, total);
        }

        [HttpGet("area")]
        public async Task<ActionResult<ApiResult>> GetInvitationByAreaId(int id, int page, int size)
        {
            RefAsync<int> total = 0;
            var data = await _iInvitationService.QueryDTOAsync<InvitationDTOView>(it => it.AreaId == id, page, size, total);
            if (data.Count == 0) return ApiResultHelper.Error("没有更多的值");
            return ApiResultHelper.Success(data, total);
        }

        [HttpGet("user")]
        public async Task<ActionResult<ApiResult>> GetInvitationByUserId(int id, int page, int size)
        {
            RefAsync<int> total = 0;
            var data = await _iInvitationService.QueryAsync(it => it.UserId == id, page, size, total);
            if (data.Count == 0) return ApiResultHelper.Error("没有更多的值");
            return ApiResultHelper.Success(data, total);
        }

        [Authorize]
        [HttpPut("view")]
        public async Task<ActionResult<ApiResult>> AddView(int id)
        {
            var invitation = await _iInvitationService.FindAsync(id);
            if (invitation == null) return ApiResultHelper.Error("没有找到该记录");

            
            invitation.Views += 1;

            bool b = await _iInvitationService.EditAsync(invitation);
            if (!b) return ApiResultHelper.Error("修改失败");
            return ApiResultHelper.Success(null);
        }
    }
}
