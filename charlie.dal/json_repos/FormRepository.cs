using charlie.dal.interfaces;
using charlie.dto.Form;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace charlie.dal.json_repos
{
    public class FormRepository : IFormRepository
    {
        private string _path;
        public FormRepository(IConfiguration config)
        {
            _path = config["FormPath"];

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
        }

        public async Task<FormModel> SaveForm(FormModel form)
        {
            if (string.IsNullOrWhiteSpace(form.formId))
            {
                form.formId = Guid.NewGuid().ToString();
            }

            await File.WriteAllTextAsync(getFormFilePath(form.formId), JsonConvert.SerializeObject(form));
            return form;
        }

        public async Task<FormModel> GetForm(string id)
        {
            var path = getFormFilePath(id);
            if (File.Exists(path))
            {
                var text = await File.ReadAllTextAsync(path);
                return JsonConvert.DeserializeObject<FormModel>(text);
            }

            return null;
        }

        public async Task<IEnumerable<FormModel>> GetAll()
        {
            List<FormModel> forms = new List<FormModel>();
            var filePaths = Directory.GetFiles(_path);
            foreach (var file in filePaths)
            {
                var data = await File.ReadAllTextAsync(file);
                try
                {
                    var form = JsonConvert.DeserializeObject<FormModel>(data);
                    forms.Add(form);
                }
                catch (Exception)
                {
                }
            }
            return forms;
        }

        private string getFormFilePath(string id)
        {
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            return string.Format("{0}{1}.json", _path, id);
        }
    }
}
