
/* 
* created by panyinglong at 2015-8-14
*/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PureMVC {
    //------------------ framework -----------------------
    #region message
    class Observer : IObserver{
		public Observer(string notifyMethod, object notifyContext){
			m_notifyMethod = notifyMethod;
			m_notifyContext = notifyContext;
		}
		public virtual void NotifyObserver(INotification notification){
			object context;
			string method;

			lock (m_syncRoot){
				context = NotifyContext;
				method = NotifyMethod;
			}

			Type t = context.GetType();
			BindingFlags f = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;
			MethodInfo mi = t.GetMethod(method, f);
			mi.Invoke(context, new object[] { notification });
		}
		public virtual bool CompareNotifyContext(object obj){
			lock (m_syncRoot){
				return NotifyContext.Equals(obj);
			}
		}
		public virtual string NotifyMethod{
			private get{
				return m_notifyMethod;
			}set{
				m_notifyMethod = value;
			}
		}
		public virtual object NotifyContext{
			private get{
				return m_notifyContext;
			}set{
				m_notifyContext = value;
			}
		}
		private string m_notifyMethod;
		private object m_notifyContext;
		protected readonly object m_syncRoot = new object();
	}
    class Notification : INotification{
		private string m_name;
		private string m_type;
		private object m_body;

        public Notification(string name) : this(name, null, null){ 
        }

        public Notification(string name, object body) : this(name, body, null){ 
        }

        public Notification(string name, object body, string type){
			m_name = name;
			m_body = body;
			m_type = type;
		}

		public virtual string Name{
			get { return m_name; }
		}
		
		public virtual object Body{
			get{
				return m_body;
			}set{
				m_body = value;
			}
		}
		
		public virtual string Type{
			get{
				return m_type;
			}set{
				m_type = value;
			}
		}
	}
    #endregion

    // mediator
    public class Mediator : IMediator, INotifier{
		public const string NAME = "Mediator";
        protected string m_mediatorName;
        protected object m_viewComponent;

        public Mediator()
            : this(NAME, null){
        }
        public Mediator(string mediatorName)
            : this(mediatorName, null){
		}
		public Mediator(string mediatorName, object viewComponent){
			m_mediatorName = (mediatorName != null) ? mediatorName : NAME;
			m_viewComponent = viewComponent;
		}
		public virtual string MediatorName{
			get { return m_mediatorName; }
		}
		public virtual object ViewComponent{
			get { return m_viewComponent; }
			set { m_viewComponent = value; }
		}
        public virtual void SendNotification(string notificationName) {
			Facade.Instance.SendNotification(notificationName);
		}

		public virtual void SendNotification(string notificationName, object body){
			Facade.Instance.SendNotification(notificationName, body);
		}

		public virtual void SendNotification(string notificationName, object body, string type){
            Facade.Instance.SendNotification(notificationName, body, type);
		}
		public virtual IList<string> ListNotificationInterests(){
			return new List<string>();
		}
		public virtual void HandleNotification(INotification notification){
		}
		public virtual void OnRegister(){
		}
		public virtual void OnRemove(){
		}
	}

    #region interface
    public interface INotification
    {
		string Name { get; }
		object Body { get; set; }
        string Type { get; set; }
    }
    public interface ICommand
    {
		void Execute(INotification notification);
    }
    public interface IObserver
    {
		string NotifyMethod { set; }
		object NotifyContext { set; }
        void NotifyObserver(INotification notification);
		bool CompareNotifyContext(object obj);
    }
    public interface IProxy
    {
		string ProxyName { get; }
		object Data { get; set; }
		void OnRegister();
		void OnRemove();
    }
    public interface IMediator
	{
		string MediatorName { get; }
		object ViewComponent { get; set; }
        IList<string> ListNotificationInterests();
		void HandleNotification(INotification notification);
		void OnRegister();
		void OnRemove();
	}
    public interface INotifier
    {
		void SendNotification(string notificationName);
		void SendNotification(string notificationName, object body);
		void SendNotification(string notificationName, object body, string type);
    }
    public interface IController
    {
        void RegisterCommand(string notificationName, Type commandType);
		void ExecuteCommand(INotification notification);
		void RemoveCommand(string notificationName);
		//bool HasCommand(string notificationName);
	}
    public interface IFacade : INotifier
	{
		void RegisterProxy(IProxy proxy);
		IProxy RetrieveProxy(string proxyName);
        IProxy RemoveProxy(string proxyName);
		//bool HasProxy(string proxyName);
        void RegisterCommand(string notificationName, Type commandType);
		void RemoveCommand(string notificationName);
		//bool HasCommand(string notificationName);
		void RegisterMediator(IMediator mediator);
		IMediator RetrieveMediator(string mediatorName);
        IMediator RemoveMediator(string mediatorName);
		//bool HasMediator(string mediatorName);
		//void NotifyObservers(INotification note);
	}
    public interface IModel
    {
		void RegisterProxy(IProxy proxy);
		IProxy RetrieveProxy(string proxyName);
        IProxy RemoveProxy(string proxyName);
		//bool HasProxy(string proxyName);
    }
    public interface IView
	{
		void RegisterObserver(string notificationName, IObserver observer);
		void RemoveObserver(string notificationName, object notifyContext);
		void NotifyObservers(INotification note);
		void RegisterMediator(IMediator mediator);
		IMediator RetrieveMediator(string mediatorName);
        IMediator RemoveMediator(string mediatorName);
		//bool HasMediator(string mediatorName);
	}
    #endregion
    
    #region M/V/C sigleton
    public class Controller : IController {
        private static Controller _instance;
        private Dictionary<string, Type> m_commandMap;
        private static readonly object m_syncRoot = new object();

        private Controller() {            
			m_commandMap = new Dictionary<string, Type>();	
        }
        public static Controller Intance{
            get{
                if (_instance == null) {
                    _instance = new Controller();
                }
                return _instance;
            }
        }
        public void RegisterCommand(string notificationName, Type commandType) { 
            lock (m_syncRoot){
                if (!m_commandMap.ContainsKey(notificationName)){   // 监听SendNotification(notificationName)，监听到则执行this.executeCommand
				    View.Instance.RegisterObserver(notificationName, new Observer("executeCommand", this));
			    }
				m_commandMap[notificationName] = commandType;
			}
        }
		public void RemoveCommand(string notificationName){
            lock (m_syncRoot){
				if (m_commandMap.ContainsKey(notificationName)){
					m_commandMap.Remove(notificationName);
				}
			}
        }
        public void ExecuteCommand(INotification notification) {
            Type commandType = null;

			lock (m_syncRoot){
				if (!m_commandMap.ContainsKey(notification.Name)) 
                    return;
				commandType = m_commandMap[notification.Name];
			}
            object commandInstance = Activator.CreateInstance(commandType); // 反射创建对象
			if (commandInstance is ICommand){
				((ICommand) commandInstance).Execute(notification);
			}
        }
    }   
    public class View : IView {
		private IDictionary<string, IMediator> m_mediatorMap;
		private IDictionary<string, IList<IObserver>> m_observerMap;
		private static View _instance;
		protected readonly object m_syncRoot = new object();
		protected static readonly object m_staticSyncRoot = new object();

		private View(){
			m_mediatorMap = new Dictionary<string, IMediator>();
			m_observerMap = new Dictionary<string, IList<IObserver>>();
		}
        public static View Instance{
            get{
                if (_instance == null) {
                    _instance = new View();
                }
                return _instance;
            }
        }
		public virtual void RegisterObserver(string notificationName, IObserver observer){
			lock (m_syncRoot){
				if (!m_observerMap.ContainsKey(notificationName)){
					m_observerMap[notificationName] = new List<IObserver>();
				}
				m_observerMap[notificationName].Add(observer);
			}
		}
		public virtual void NotifyObservers(INotification notification){
			IList<IObserver> observers = null;
			lock (m_syncRoot){
				if (m_observerMap.ContainsKey(notification.Name)){
					IList<IObserver> observers_ref = m_observerMap[notification.Name];
					observers = new List<IObserver>(observers_ref);
				}
			}
			if (observers != null){	
				for (int i = 0; i < observers.Count; i++){
					IObserver observer = observers[i];
					observer.NotifyObserver(notification);
				}
			}
		}
		public virtual void RemoveObserver(string notificationName, object notifyContext){
			lock (m_syncRoot){
				if (m_observerMap.ContainsKey(notificationName)){
					IList<IObserver> observers = m_observerMap[notificationName];
					for (int i = 0; i < observers.Count; i++){
						if (observers[i].CompareNotifyContext(notifyContext)){
							observers.RemoveAt(i);
							break;
						}
					}
					if (observers.Count == 0){
						m_observerMap.Remove(notificationName);
					}
				}
			}
		}
		public virtual void RegisterMediator(IMediator mediator){
			lock (m_syncRoot){
				if (m_mediatorMap.ContainsKey(mediator.MediatorName)) 
                    return;

				m_mediatorMap[mediator.MediatorName] = mediator;

				IList<string> interests = mediator.ListNotificationInterests();
				if (interests.Count > 0){
					IObserver observer = new Observer("handleNotification", mediator);  // 由mediator接收消息并做出回应
					for (int i = 0; i < interests.Count; i++){
						RegisterObserver(interests[i].ToString(), observer);
					}
				}
			}
			mediator.OnRegister();
		}
		public virtual IMediator RetrieveMediator(string mediatorName){
			lock (m_syncRoot){
				if (!m_mediatorMap.ContainsKey(mediatorName)) return null;
				return m_mediatorMap[mediatorName];
			}
		}
		public virtual IMediator RemoveMediator(string mediatorName){
			IMediator mediator = null;

			lock (m_syncRoot){
				if (!m_mediatorMap.ContainsKey(mediatorName)) return null;
				mediator = (IMediator) m_mediatorMap[mediatorName];

				IList<string> interests = mediator.ListNotificationInterests();
				for (int i = 0; i < interests.Count; i++){
					RemoveObserver(interests[i], mediator);
				}
				m_mediatorMap.Remove(mediatorName);
			}
			if (mediator != null) mediator.OnRemove();
			return mediator;
		}
    }
    public class Model : IModel 
    {
		protected IDictionary<string, IProxy> m_proxyMap;
		protected static Model _instance;
		protected readonly object m_syncRoot = new object();
		protected static readonly object m_staticSyncRoot = new object();
		private Model(){
			m_proxyMap = new Dictionary<string, IProxy>();
		}
        public static Model Instance{
			get{
				if (_instance == null){
					_instance = new Model();
				}
				return _instance;
			}
		}
		public virtual void RegisterProxy(IProxy proxy){
			lock (m_syncRoot){
				m_proxyMap[proxy.ProxyName] = proxy;
			}

			proxy.OnRegister();
		}
		public virtual IProxy RetrieveProxy(string proxyName){
			lock (m_syncRoot){
				if (!m_proxyMap.ContainsKey(proxyName)) return null;
				return m_proxyMap[proxyName];
			}
		}
		public virtual IProxy RemoveProxy(string proxyName){
			IProxy proxy = null;
			lock (m_syncRoot){
				if (m_proxyMap.ContainsKey(proxyName)){
					proxy = RetrieveProxy(proxyName);
					m_proxyMap.Remove(proxyName);
				}
			}
			if (proxy != null) proxy.OnRemove();
			return proxy;
		}
    }
    public class Facade : IFacade 
    {
        private static Facade _instance;

        private Facade(){}

        public static Facade Instance{
            get{
                if (_instance == null) {
                    _instance = new Facade();
                }
                return _instance;
            }
        }
        public void RegisterCommand(string notificationName, Type commandType){
            Controller.Intance.RegisterCommand(notificationName, commandType);
        }
        public virtual void RemoveCommand(string notificationName){
			Controller.Intance.RemoveCommand(notificationName);
		}


        public void RegisterProxy(IProxy proxy){
			Model.Instance.RegisterProxy(proxy);
		}
        public virtual IProxy RetrieveProxy(string proxyName){
			return Model.Instance.RetrieveProxy(proxyName);
		}
        public IProxy RemoveProxy(string proxyName){
			return Model.Instance.RemoveProxy(proxyName);
		}

        public void RegisterMediator(IMediator mediator){
			View.Instance.RegisterMediator(mediator);
		}
        public virtual IMediator RetrieveMediator(string mediatorName){
			return View.Instance.RetrieveMediator(mediatorName);
		}
        public virtual IMediator RemoveMediator(string mediatorName){
			return View.Instance.RemoveMediator(mediatorName);
		}

        public void SendNotification(string notificationName){
			View.Instance.NotifyObservers(new Notification(notificationName));
		}
        public void SendNotification(string notificationName, object body){
			View.Instance.NotifyObservers(new Notification(notificationName, body));
		}
        public void SendNotification(string notificationName, object body, string type){
			View.Instance.NotifyObservers(new Notification(notificationName, body, type));
		}
    }
    #endregion
    //---------------------------- end ------------------------------
}

namespace PureMVCDemoSimplify {
    using PureMVC;

    class test {
        static void Main()
        {
            Facade.Instance.RegisterCommand(NotiConst.START_UP, typeof(StartUpCommand));
            Facade.Instance.RegisterCommand(NotiConst.LOAD_UI, typeof(LoadUICommand));
            Facade.Instance.RegisterCommand(NotiConst.LOGIN, typeof(LoginCommand));
            Facade.Instance.RegisterCommand(NotiConst.CLOSE, typeof(CloseCommand));

            Facade.Instance.SendNotification(NotiConst.START_UP);
            Facade.Instance.RemoveCommand(NotiConst.START_UP);
   
            Console.Read();
        }
    }
    
    #region logic

    public class NotiConst{
        public const string START_UP = "StartUp";                       //启动框架
        public const string LOAD_UI = "LoadUI";                         //初始化UI
        public const string LOGIN = "Login";                            //登录
        public const string CLOSE = "Close";                            //关闭UI
    }

    public class myView {
        public myView() { 
        }
        public void ShowUI() {            
            Console.WriteLine("welcome to pureMVC demo. print 'L' to login; other to close.");
            ConsoleKeyInfo c = Console.ReadKey();
            Console.WriteLine();
            if (c.KeyChar == 'L' || c.KeyChar == 'l') {
                Facade.Instance.SendNotification(NotiConst.LOGIN);
                Facade.Instance.RemoveCommand(NotiConst.LOGIN);
            } else {
                Facade.Instance.SendNotification(NotiConst.CLOSE);
                Facade.Instance.RemoveCommand(NotiConst.CLOSE);
            }
        }
        public void OnLogin(INotification notification) { 
            Console.WriteLine("login succeed!");
        }
        public void OnClose(INotification notification) {  
            Console.WriteLine("closed!");
        }
    }
    public class myMediator : Mediator {
        public const string NAME = "myMediator";
        public myMediator(object viewComponent) : base(NAME, viewComponent) {
        }
        public myView view {
            get {
                return ViewComponent as myView;
            }
        }
        // AppMediator需要监听的消息
        override public IList<string> ListNotificationInterests() {
            return new List<string>()
            { 
                NotiConst.LOGIN,
                NotiConst.CLOSE
            };
        }
        override public void HandleNotification(INotification notification) {
            string name = notification.Name;
            switch (name) {
                case NotiConst.LOGIN:      //登录
                    view.OnLogin(notification);
                break;
                case NotiConst.CLOSE:      //关闭
                    view.OnClose(notification);
                break;
            }
        }
    }

    public class StartUpCommand : ICommand {
        public void Execute(INotification notification) {
            Console.WriteLine("excete StartUpCommand..");
            Facade.Instance.SendNotification(NotiConst.LOAD_UI);
            Facade.Instance.RemoveCommand(NotiConst.LOAD_UI);
        }
    }
    public class LoadUICommand : ICommand {
        public void Execute(INotification notification) {  
            Console.WriteLine("excete LoadUICommand..");                    
            myView view = new myView();
            Facade.Instance.RegisterMediator(new myMediator(view));
            view.ShowUI();
        }
    }
    public class LoginCommand : ICommand {
        public void Execute(INotification notification) { 
            Console.WriteLine("excete LoginCommand..");
        }
    }
    public class CloseCommand : ICommand {
        public void Execute(INotification notification) { 
            Console.WriteLine("excete CloseCommand..");
        }
    }
    
    #endregion

}
