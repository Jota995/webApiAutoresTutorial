namespace WebApiAutores.Services
{
    public class WriteInFile : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string FileName = "File_01.txt";
        private Timer timer;

        public WriteInFile(IWebHostEnvironment env)
        {
            this.env = env;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork,null,TimeSpan.Zero, TimeSpan.FromSeconds(5));
            Write("Start Proccess");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            Write("Finish Proccess");
            return Task.CompletedTask;
        }

        private void Write(string text)
        {
            var route = $@"{env.ContentRootPath}\wwwroot\{FileName}";

            using(StreamWriter writer = new StreamWriter(route, append: true))
            {
                writer.WriteLine(text);
            }
        }

        private void DoWork(object state) 
        {
            Write($@"Proceso en ejecuccion {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")} ");
        }
    }
}
