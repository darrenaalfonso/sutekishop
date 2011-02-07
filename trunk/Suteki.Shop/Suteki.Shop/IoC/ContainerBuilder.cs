using System.Reflection;
using System.Web.Mvc;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Microsoft.Practices.ServiceLocation;
using Suteki.Common.Binders;
using Suteki.Common.Events;
using Suteki.Common.Filters;
using Suteki.Common.HtmlHelpers;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Windsor;
using Suteki.Shop.Binders;
using Suteki.Shop.Filters;
using Suteki.Shop.Services;

namespace Suteki.Shop.IoC
{
    public static class ContainerBuilder
    {
        public static IWindsorContainer Build(string configPath)
        {
            var container = new WindsorContainer(new XmlInterpreter(configPath));

            // add array resolver
            container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel));

            // register handler selectors
            container.Kernel.AddHandlerSelector(new UrlBasedComponentSelector(
                                                    typeof(IBaseControllerService),
                                                    typeof(IImageFileService),
                                                    typeof(IEmailSender)
                                                    ));

            // automatically register controllers
            container.Register(AllTypes
                                   .FromAssembly(Assembly.GetExecutingAssembly())
                                   .BasedOn<IController>()
                                   .Configure(c => c.LifeStyle.Transient.Named(c.Implementation.Name.ToLower())));

            container.Register(
                AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
                        .BasedOn(typeof(IHandle<>)).WithService.Base()
                        .Configure(c => c.LifeStyle.Transient),
                Component.For<ILogger>().ImplementedBy<Logger>().LifeStyle.Transient,
                Component.For(typeof(IRepository<>)).ImplementedBy(typeof(NHibernateRepository<>)).LifeStyle.Transient,
                Component.For(typeof(IComboFor<,>)).ImplementedBy(typeof(ComboFor<,>)).LifeStyle.Transient,
                Component.For<IImageService>().ImplementedBy<ImageService>().Named("image.service").LifeStyle.Transient,
                Component.For<IEncryptionService>().ImplementedBy<EncryptionService>().Named("encryption.service").LifeStyle.Transient,
                Component.For<IHttpFileService>().ImplementedBy<HttpFileService>().LifeStyle.Transient,
                Component.For<ISizeService>().ImplementedBy<SizeService>().LifeStyle.Transient,
                Component.For<IUserService>().ImplementedBy<UserService>().LifeStyle.Transient,
                Component.For(typeof(IOrderableService<>)).ImplementedBy(typeof(OrderableService<>)).LifeStyle.Transient,
                Component.For<IPostageService>().ImplementedBy<PostageService>().LifeStyle.Transient,
                Component.For<IRepositoryResolver>().ImplementedBy<RepositoryResolver>().LifeStyle.Transient,
                Component.For<IHttpContextService>().ImplementedBy<HttpContextService>().LifeStyle.Transient,
                Component.For<IUnitOfWorkManager>().ImplementedBy<UnitOfWorkManager>().LifeStyle.Transient,
                Component.For<IFormsAuthentication>().ImplementedBy<FormsAuthenticationWrapper>(),
                Component.For<IServiceLocator>().Instance(new WindsorServiceLocator(container)),
                Component.For<AuthenticateFilter>().LifeStyle.Transient,
                Component.For<UnitOfWorkFilter>().LifeStyle.Transient,
                Component.For<CurrentBasketBinder>().LifeStyle.Transient,
                Component.For<ProductBinder>().LifeStyle.Transient,
                Component.For<EnsureSsl>().LifeStyle.Transient,
				Component.For<MailingListSubscriptionBinder>().LifeStyle.Transient,
                Component.For<IOrderSearchService>().ImplementedBy<OrderSearchService>().LifeStyle.Transient,
                Component.For<IEmailBuilder>().ImplementedBy<EmailBuilder>().LifeStyle.Singleton,
                Component.For<IAppSettings>().ImplementedBy<AppSettings>().LifeStyle.Singleton,
                Component.For<IEmailService>().ImplementedBy<EmailService>().LifeStyle.Transient,
                Component.For<IModelBinder, EntityModelBinder>().ImplementedBy<EntityModelBinder>().LifeStyle.Transient,
                Component.For<IBasketService>().ImplementedBy<BasketService>().LifeStyle.Transient,
                Component.For<ICheckoutService>().ImplementedBy<CheckoutService>().LifeStyle.Transient,
                Component.For<IProductCopyService>().ImplementedBy<ProductCopyService>().LifeStyle.Transient
                );

            return container;
        }
    }
}