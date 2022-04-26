using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IssueLog.API.Dtos;
using IssueLog.API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace IssueLog.API.Data
{
    public class IssueFileRepository : IIssueFileRepository
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public IssueFileRepository(DataContext context, IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
        }
        public async Task<IssueFile> AddIssueFile(IssueFileToAddDto issueFileToAdd)
        {
            string extension = Path.GetExtension(issueFileToAdd.File.FileName);
            IssueFile issueFile = new IssueFile()
            {
                IssueId = issueFileToAdd.IssueId,
                Description = issueFileToAdd.File.FileName
            };
            var issueFileAdded = await _context.IssueFiles.AddAsync(issueFile);
            await _context.SaveChangesAsync();
            issueFile.FileUrl = issueFile.Id + '.' + extension;
            await _context.SaveChangesAsync();
            var issue = await _context.Issues.Where(x => x.Id == issueFile.IssueId).FirstOrDefaultAsync();
            var webRoot = _env.WebRootPath;
            try
            {
                string mainPath = Path.Combine(webRoot,"..");
                string folderPath = Path.Combine(mainPath, "GI-EC_NC_FILES", issue.IssueNo);
                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                }
                string path = Path.Combine(folderPath, issueFile.FileUrl);
                var file = issueFileToAdd.File;
                using (var fileStream = new FileStream(@path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                throw new Exception("Failured to save files");
            }

            return issueFile;
        }

        public async Task<bool> DeleteIssueFile(int id)
        {
            var issueFileInDB = await _context.IssueFiles.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (issueFileInDB == null)
            {
                throw new Exception("File does not exists in the database.");
            }
            var issue = await _context.Issues.Where(x => x.Id == issueFileInDB.IssueId).FirstOrDefaultAsync();
            if (issue == null)
            {
                throw new Exception("Issue does not exists in the database.");
            }
            try
            {
                var webRoot = _env.WebRootPath;
                string mainPath = Path.Combine(webRoot,"..");
                string folderPath = Path.Combine(mainPath,  "GI-EC_NC_FILES", issue.IssueNo);

                string path = Path.Combine(folderPath, issueFileInDB.FileUrl);
                if (!System.IO.File.Exists(path))
                {
                    throw new Exception("File does not exist!");
                }
                System.IO.File.Delete (path);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                throw new Exception("Failured to delete file!");
            }
            _context.IssueFiles.Remove(issueFileInDB);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<IssueFile>> GetIssueFileById(int issueId)
        {
            var issueFiles = await _context.IssueFiles.Where(x => x.IssueId == issueId).ToListAsync();
            return issueFiles;
        }

    }
}