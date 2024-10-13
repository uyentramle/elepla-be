﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository AccountRepository { get; }
        IRoleRepository RoleRepository { get; }
        IImageRepository ImageRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IArticleRepository ArticleRepository { get; }
        IArticleCategoryRepository ArticleCategoryRepository { get; }
        IArticleImageRepository ArticleImageRepository { get; }
        IServicePackageRepository ServicePackageRepository { get; }
        IUserPackageRepository UserPackageRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        IQuestionBankRepository QuestionBankRepository { get; }
        ISubjectRepository SubjectRepository { get; }
        ICurriculumFrameworkRepository CurriculumFrameworkRepository { get; }
        IGradeRepository GradeRepository { get; }
        ISubjectInCurriculumRepository SubjectInCurriculumRepository { get; }
        ILessonRepository LessonRepository { get; }
        IPlanbookCollectionRepository PlanbookCollectionRepository { get; }
        IPlanbookRepository PlanbookRepository { get; }
        IActivityRepository ActivityRepository { get; }
        ITeachingScheduleRepository TeachingScheduleRepository { get; }
        IFeedbackRepository FeedbackRepository { get; }
        Task<int> SaveChangeAsync();
    }
}
