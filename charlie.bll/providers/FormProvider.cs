using charlie.bll.interfaces;
using charlie.dal.interfaces;
using charlie.dto.Form;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.bll.providers
{
    public class FormProvider : IFormProvider
    {
        private IFormRepository _formRepo;
        private ILogWriter _logger;

        public FormProvider(IFormRepository formRepo, ILogWriter logger)
        {
            _formRepo = formRepo;
            _logger = logger;
        }

        public async Task<string> CreateForm(FormModel newForm)
        {
            _logger.ServerLogInfo("Creating form");

            try
            {
                var form = await _formRepo.SaveForm(newForm);
                return form.formId;
            }
            catch(Exception e)
            {
                _logger.ServerLogError(e.Message);
                return string.Empty;
            }
        }

        public async Task<bool> UpdateForm(FormModel data)
        {
            _logger.ServerLogInfo("Update form, calling into create form");
            try
            {
                return !string.IsNullOrEmpty(await CreateForm(data));
            }
            catch(Exception e)
            {
                _logger.ServerLogError(e.Message);
                return false;
            }
        }

        public async Task<FormModel> GetById(string id)
        {
            _logger.ServerLogInfo("Getting form {0}", id);
            var form = await _formRepo.GetForm(id);

            if (form == null)
                return null;

            foreach (var section in form.sections)
            {
                foreach (var question in section.questions)
                {
                    question.answerContent = getAnswerContent(question.questionType, question.jsonAnswerContent);
                    question.jsonAnswerContent = string.Empty;
                }
            }
            return form;
        }

        public async Task<IEnumerable<FormModel>> GetAll()
        {
            _logger.ServerLogInfo("Getting all forms");
            var forms = await _formRepo.GetAll();
            foreach (var form in forms)
            {
                foreach (var section in form.sections)
                {
                    foreach (var question in section.questions)
                    {
                        question.answerContent = getAnswerContent(question.questionType, question.jsonAnswerContent);
                        question.jsonAnswerContent = string.Empty;
                    }
                }
            }
            return forms;
        }

        public AnswerContent getAnswerContent(string questionType, string json)
        {
            questionType = questionType?.Trim().ToLower();
            switch (questionType)
            {
                case "short answer":
                case "paragraph":
                    return JsonConvert.DeserializeObject<TextQuestionContent>(json);
                case "multiple choice":
                case "checkboxes":
                case "dropdown":
                    return JsonConvert.DeserializeObject<ListAnswerContent>(json);
                case "file upload":
                    return JsonConvert.DeserializeObject<FileQuestionContent>(json);
                case "linear scale":
                    return JsonConvert.DeserializeObject<ScaleQuestionContent>(json);
                case "multiple choice grid":
                case "checkbox grid":
                    return JsonConvert.DeserializeObject<GridQuestionContent>(json);
                case "date":
                    return JsonConvert.DeserializeObject<DateQuestionContent>(json);
                case "time":
                    return JsonConvert.DeserializeObject<TimeQuestionContent>(json);
                case "location":
                    return JsonConvert.DeserializeObject<LocationQuestionContent>(json);
                default:
                    _logger.ServerLogWarning("Unknown AnswerContent in FormProvider getAnswerContent");
                    return JsonConvert.DeserializeObject<TextQuestionContent>(json);
            }
        }
    }
}
