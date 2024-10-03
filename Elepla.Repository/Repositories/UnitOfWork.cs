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
            IQuestionBankRepository questionBankRepository)
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
