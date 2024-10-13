using Elepla.Repository.Data;
using Elepla.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IArticleCategoryRepository _articleCategoryRepository;
        private readonly IArticleImageRepository _articleImageRepository;
        private readonly IServicePackageRepository _servicePackageRepository;
        private readonly IUserPackageRepository _userPackageRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IQuestionBankRepository _questionBankRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ICurriculumFrameworkRepository _curriculumFrameworkRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly ISubjectInCurriculumRepository _subjectInCurriculumRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IPlanbookCollectionRepository _planbookCollectionRepository;
        private readonly IPlanbookRepository _planbookRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly ITeachingScheduleRepository _teachingScheduleRepository;
        private readonly IFeedbackRepository _feedbackRepository;

        public UnitOfWork(AppDbContext dbContext,
            IAccountRepository accountRepository,
            IRoleRepository roleRepository,
            IImageRepository imageRepository,
            ICategoryRepository categoryRepository,
            IArticleRepository articleRepository,
            IArticleCategoryRepository articleCategoryRepository,
            IArticleImageRepository articleImageRepository,
            IServicePackageRepository servicePackageRepository,
            IUserPackageRepository userPackageRepository,
            IPaymentRepository paymentRepository,
            IQuestionBankRepository questionBankRepository,
            ISubjectRepository subjectRepository,
            ICurriculumFrameworkRepository curriculumFrameworkRepository,
            IGradeRepository gradeRepository,
            ISubjectInCurriculumRepository subjectInCurriculumRepository,
            ILessonRepository lessonRepository,
            IPlanbookCollectionRepository planbookCollectionRepository,
            IPlanbookRepository planbookRepository,
            IActivityRepository activityRepository,
            ITeachingScheduleRepository teachingScheduleRepository,
            IFeedbackRepository feedbackRepository)
        {
            _dbContext = dbContext;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _imageRepository = imageRepository;
            _categoryRepository = categoryRepository;
            _articleRepository = articleRepository;
            _articleCategoryRepository = articleCategoryRepository;
            _articleImageRepository = articleImageRepository;
            _servicePackageRepository = servicePackageRepository;
            _userPackageRepository = userPackageRepository;
            _paymentRepository = paymentRepository;
            _questionBankRepository = questionBankRepository;
            _subjectRepository = subjectRepository;
            _curriculumFrameworkRepository = curriculumFrameworkRepository;
            _gradeRepository = gradeRepository;
            _subjectInCurriculumRepository = subjectInCurriculumRepository;
            _lessonRepository = lessonRepository;
            _planbookCollectionRepository = planbookCollectionRepository;
            _planbookRepository = planbookRepository;
            _activityRepository = activityRepository;
            _teachingScheduleRepository = teachingScheduleRepository;
            _feedbackRepository = feedbackRepository;
        }

        public IAccountRepository AccountRepository => _accountRepository;

        public IRoleRepository RoleRepository => _roleRepository;

        public IImageRepository ImageRepository => _imageRepository;

        public ICategoryRepository CategoryRepository => _categoryRepository;

        public IArticleRepository ArticleRepository => _articleRepository;

        public IArticleCategoryRepository ArticleCategoryRepository => _articleCategoryRepository;

        public IArticleImageRepository ArticleImageRepository => _articleImageRepository;

        public IServicePackageRepository ServicePackageRepository => _servicePackageRepository;

        public IUserPackageRepository UserPackageRepository => _userPackageRepository;

        public IPaymentRepository PaymentRepository => _paymentRepository;

        public IQuestionBankRepository QuestionBankRepository => _questionBankRepository;

        public ISubjectRepository SubjectRepository => _subjectRepository;

        public ICurriculumFrameworkRepository CurriculumFrameworkRepository => _curriculumFrameworkRepository;

        public IGradeRepository GradeRepository => _gradeRepository;

        public ISubjectInCurriculumRepository SubjectInCurriculumRepository => _subjectInCurriculumRepository;

        public ILessonRepository LessonRepository => _lessonRepository;

        public IPlanbookCollectionRepository PlanbookCollectionRepository => _planbookCollectionRepository;

        public IPlanbookRepository PlanbookRepository => _planbookRepository;

        public IActivityRepository ActivityRepository => _activityRepository;

        public ITeachingScheduleRepository TeachingScheduleRepository => _teachingScheduleRepository;

        public IFeedbackRepository FeedbackRepository => _feedbackRepository;

        public async Task<int> SaveChangeAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
