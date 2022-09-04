﻿using HearthStoneForum.IRepository;
using HearthStoneForum.Model;
using HearthStoneForum.Model.Dto;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HearthStoneForum.Repository
{
    public class InvitationRepository : BaseRepository<Invitation>, IInvitationRepository
    {
        public async Task<List<InvitationDTORecommend>> GetRecommendInvitations()
        {
            var data = await base.Context.Queryable<Invitation>()
                .Select(it => new InvitationDTORecommend()
                {
                    Title = it.Title,
                    Id = it.Id
                })
                .Mapper(it => it.LikesCount = Context.Queryable<Likes>().Where(l => l.InvitationId == it.Id).Count())
                .Mapper(it => it.CommentCount = Context.Queryable<Comment>().Where(c1 => c1.InvitationId == it.Id).Count())
                .Mapper(it => it.CollectionCount = Context.Queryable<Collection>().Where(c2 => c2.InvitationId == it.Id).Count())
                .Mapper(it => it.Recommend = it.LikesCount + it.CommentCount + it.CollectionCount)

                .ToListAsync();
            return data.OrderByDescending(it => it.Recommend).ToList();
        }


        public async Task<List<InvitationDTORecommend>> GetNewInvitations()
        {
            return await base.Context.Queryable<Invitation>()
                .OrderByDescending(it => it.CreatedTime)
                .Select(it => new InvitationDTORecommend()
                {
                    Id = it.Id,
                    Title = it.Title
                })
                .Take(50)
                .ToListAsync();
        }
        public override Task<List<DTO>> QueryDTOAsync<DTO>()
        {
            var LikeCount = Context.Queryable<Likes>()
                .GroupBy(it => it.InvitationId)
                .Select(it => new
                {
                    InvitationId = it.InvitationId,
                    Count = SqlFunc.AggregateCount(it.Id)
                });
            var CommentCount = Context.Queryable<Comment>()
                .GroupBy(it => it.InvitationId)
                .Select(it => new
                {
                    InvitationId = it.InvitationId,
                    Count = SqlFunc.AggregateCount(it.Id)
                });
            var CollectionCount = Context.Queryable<Collection>()
                .GroupBy(it => it.InvitationId)
                .Select(it => new
                {
                    InvitationId = it.InvitationId,
                    Count = SqlFunc.AggregateCount(it.Id)
                });

            return base.Context.Queryable<Invitation>()
                .LeftJoin<Area>((i, a) => i.AreaId == a.Id)
                .LeftJoin<UserInfo>((i, a, u) => i.UserId == u.Id)
                .LeftJoin(LikeCount, (i, a, u, l) => i.Id == l.InvitationId)
                .LeftJoin(CommentCount, (i, a, u, l, c1) => i.Id == c1.InvitationId)
                .LeftJoin(CollectionCount, (i, a, u, l, c1, c2) => i.Id == c2.InvitationId)

                .Select((i, a, u, l, c1, c2) => new InvitationDTO()
                {
                    Id = i.Id,
                    AreaId = i.AreaId,
                    AreaName = a.Name,
                    Title = i.Title,
                    Content = i.Content,
                    UserId = i.UserId,
                    UserName = u.UserName,
                    Views = i.Views,
                    ImagePaths = i.ImagePaths,
                    CreatedTime = i.CreatedTime,
                    LikeCount = l.Count,
                    CommentCount = c1.Count,
                    CollectionCount = c2.Count
                })
                .MergeTable()
                .ToListAsync(it => new DTO());
        }
        public override Task<List<DTO>> QueryDTOAsync<DTO>(Expression<Func<DTO, bool>> func)
        {
            var LikeCount = Context.Queryable<Likes>()
                .GroupBy(it => it.InvitationId)
                .Select(it => new
                {
                    InvitationId = it.InvitationId,
                    Count = SqlFunc.AggregateCount(it.Id)
                });
            var CommentCount = Context.Queryable<Comment>()
                .GroupBy(it => it.InvitationId)
                .Select(it => new
                {
                    InvitationId = it.InvitationId,
                    Count = SqlFunc.AggregateCount(it.Id)
                });
            var CollectionCount = Context.Queryable<Collection>()
                .GroupBy(it => it.InvitationId)
                .Select(it => new
                {
                    InvitationId = it.InvitationId,
                    Count = SqlFunc.AggregateCount(it.Id)
                });

            return base.Context.Queryable<Invitation>()
                .LeftJoin<Area>((i, a) => i.AreaId == a.Id)
                .LeftJoin<UserInfo>((i, a, u) => i.UserId == u.Id)
                .LeftJoin(LikeCount, (i, a, u, l) => i.Id == l.InvitationId)
                .LeftJoin(CommentCount, (i, a, u, l, c1) => i.Id == c1.InvitationId)
                .LeftJoin(CollectionCount, (i, a, u, l, c1, c2) => i.Id == c2.InvitationId)

                .Select((i, a, u, l, c1, c2) => new InvitationDTO()
                {
                    Id = i.Id,
                    AreaId = i.AreaId,
                    AreaName = a.Name,
                    Title = i.Title,
                    Content = i.Content,
                    UserId = i.UserId,
                    UserName = u.UserName,
                    Views = i.Views,
                    ImagePaths = i.ImagePaths,
                    CreatedTime = i.CreatedTime,
                    LikeCount = l.Count,
                    CommentCount = c1.Count,
                    CollectionCount = c2.Count
                })
                .MergeTable()
                .Where(func as Expression<Func<InvitationDTO, bool>>)
                .ToListAsync(it => new DTO());
        }

        public override Task<List<DTO>> QueryDTOAsync<DTO>(int page, int size, RefAsync<int> total)
        {
            var LikeCount = Context.Queryable<Likes>()
                .GroupBy(it => it.InvitationId)
                .Select(it => new
                {
                    InvitationId = it.InvitationId,
                    Count = SqlFunc.AggregateCount(it.Id)
                });
            var CommentCount = Context.Queryable<Comment>()
                .GroupBy(it => it.InvitationId)
                .Select(it => new
                {
                    InvitationId = it.InvitationId,
                    Count = SqlFunc.AggregateCount(it.Id)
                });
            var CollectionCount = Context.Queryable<Collection>()
                .GroupBy(it => it.InvitationId)
                .Select(it => new
                {
                    InvitationId = it.InvitationId,
                    Count = SqlFunc.AggregateCount(it.Id)
                });

            return base.Context.Queryable<Invitation>()
                .LeftJoin<Area>((i, a) => i.AreaId == a.Id)
                .LeftJoin<UserInfo>((i, a, u) => i.UserId == u.Id)
                .LeftJoin(LikeCount, (i, a, u, l) => i.Id == l.InvitationId)
                .LeftJoin(CommentCount, (i, a, u, l, c1) => i.Id == c1.InvitationId)
                .LeftJoin(CollectionCount, (i, a, u, l, c1, c2) => i.Id == c2.InvitationId)

                .Select((i, a, u, l, c1, c2) => new InvitationDTO()
                {
                    Id = i.Id,
                    AreaId = i.AreaId,
                    AreaName = a.Name,
                    Title = i.Title,
                    Content = i.Content,
                    UserId = i.UserId,
                    UserName = u.UserName,
                    Views = i.Views,
                    ImagePaths = i.ImagePaths,
                    CreatedTime = i.CreatedTime,
                    LikeCount = l.Count,
                    CommentCount = c1.Count,
                    CollectionCount = c2.Count
                })
                .MergeTable()
                .ToPageListAsync(page, size, total, it => new DTO());
        }

        public override Task<List<DTO>> QueryDTOAsync<DTO>(Expression<Func<DTO, bool>> func, int page, int size, RefAsync<int> total)
        {
            var LikeCount = Context.Queryable<Likes>()
                .GroupBy(it => it.InvitationId)
                .Select(it => new
                {
                    InvitationId = it.InvitationId,
                    Count = SqlFunc.AggregateCount(it.Id)
                });
            var CommentCount = Context.Queryable<Comment>()
                .GroupBy(it => it.InvitationId)
                .Select(it => new
                {
                    InvitationId = it.InvitationId,
                    Count = SqlFunc.AggregateCount(it.Id)
                });
            var CollectionCount = Context.Queryable<Collection>()
                .GroupBy(it => it.InvitationId)
                .Select(it => new
                {
                    InvitationId = it.InvitationId,
                    Count = SqlFunc.AggregateCount(it.Id)
                });

            return base.Context.Queryable<Invitation>()
                .LeftJoin<Area>((i, a) => i.AreaId == a.Id)
                .LeftJoin<UserInfo>((i, a, u) => i.UserId == u.Id)
                .LeftJoin(LikeCount, (i, a, u, l) => i.Id == l.InvitationId)
                .LeftJoin(CommentCount, (i, a, u, l, c1) => i.Id == c1.InvitationId)
                .LeftJoin(CollectionCount, (i, a, u, l, c1, c2) => i.Id == c2.InvitationId)

                .Select((i, a, u, l, c1, c2) => new InvitationDTO()
                {
                    Id = i.Id,
                    AreaId = i.AreaId,
                    AreaName = a.Name,
                    Title = i.Title,
                    Content = i.Content,
                    UserId = i.UserId,
                    UserName = u.UserName,
                    Views = i.Views,
                    ImagePaths = i.ImagePaths,
                    CreatedTime = i.CreatedTime,
                    LikeCount = l.Count,
                    CommentCount = c1.Count,
                    CollectionCount = c2.Count
                })
                .MergeTable()
                .Where(func as Expression<Func<InvitationDTO, bool>>)
                .ToPageListAsync(page, size, total, it => new DTO());
        }
    }
}