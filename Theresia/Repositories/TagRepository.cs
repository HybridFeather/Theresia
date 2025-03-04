using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandyControl.Controls;
using Theresia.Config;
using Theresia.Entity;
using Theresia.Repositories.Interfaces;

namespace Theresia.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TagEntity>> GetTagsByNameAsync(string name)
        {
            var query = _context.Tag.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(tag => tag.Name.Contains(name));
            }
            return await query.ToListAsync();
        }

        public async Task<bool> AddTagAsync(TagEntity tag)
        {
            TagEntity? check = await _context.Tag.FirstOrDefaultAsync(t => t.Name.Equals(tag.Name));
            if (check == null){
                _context.Tag.Add(tag);
                await _context.SaveChangesAsync();
                return true;
            }  
            return false;
        }
        public async Task<bool> RmTag(int id)
        {
            var tag = await _context.Tag.FindAsync(id);
            if (tag == null)
            { 
                return false;
            }
            _context.Tag.Remove(tag);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RmTagByNameAsync(string name)
        {
            TagEntity? check = await _context.Tag.FirstOrDefaultAsync(t => t.Name.Equals(name));
            if (check == null)
            {
                return false;
            }
            _context.Tag.Remove(check);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RmTagByMovieCodeAndTagIdAsync(string movieCode, int tagId)
        {
            MovieTagsEntity? entity = await _context.MovieTags.FirstOrDefaultAsync(mt => mt.Code == movieCode && mt.TagId == tagId);
            if (entity == null)
            {
                return false;
            }

            _context.MovieTags.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public List<TagEntity> GetTagByMovieCode(string movieCode)
        {
            List<TagEntity> result =  _context.Tag.AsQueryable()
                .Join(_context.MovieTags,
                    t => t.Id,
                    mt => mt.TagId,
                    (t, mt) => new { t, mt })
                .Where(x => x.mt.Code == movieCode).Select(x => x.t).ToList();
            return result;
        }

        public TagEntity? AddTag(TagEntity tag)
        {
            TagEntity? check = _context.Tag.FirstOrDefault(t => t.Name.Equals(tag.Name));
            if (check == null)
            {
                _context.Tag.Add(tag); 
                _context.SaveChanges();
                return tag;
            }

            return null;
        }

        public bool RemoveTag(TagEntity tag)
        {
            try
            {
                _context.Tag.Remove(tag); 
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"删除标签失败，异常信息{ex}");
                return false;
            }

            return true;
        }
    }
}
