using SmsQueueSenderService.Infrastructure.Interfaces;
using SmsQueueSenderService.Infrastructure.IoC;
using Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmsQueueSenderService.Model
{
    public class ProcessorManager : IProcessorManager
    {
        private dynamic _appSettings;

        private bool _isRunning = false;
        private int _engineSleepTime = 2000;

        private readonly IQueueList _msmqCollection;
        private List<Task> _processSmsQueueTasks;

        public ProcessorManager(IAppSettingsWrapper appSettings, IQueueList msmqCollection)
        {
            _appSettings = appSettings;

            _engineSleepTime = Convert.ToInt32(_appSettings.EngineSleepTime);

            _msmqCollection = msmqCollection;

            _isRunning = true;
        }

        public void Start(CancellationToken cancellationToken)
        {
            LogWriter.LogEntry("Entering WorkHandle loop", LoggerType.Log4Net);

            _processSmsQueueTasks = new List<Task>();

            foreach (var queue in _msmqCollection.Queues)
            {
                try
                {
                    var task = Task.Run(async () =>
                    {
                        while (_isRunning && !cancellationToken.IsCancellationRequested)
                        {
                            try
                            {
                                using (var nested = IoCBootstrapper.Instance.Container.GetNestedContainer())
                                using (var pipeline = nested.GetInstance<IMessagePipeline>())
                                {
                                    pipeline.Process(queue.Value, cancellationToken);
                                }
                            }
                            catch (Exception ex)
                            {
                                LogWriter.LogException(ex, "There was an error while processing a queue: " + ex.Message, LoggerType.Log4Net);
                            }

                            await Task.Delay(_engineSleepTime, cancellationToken);
                        }
                    }, cancellationToken);

                    _processSmsQueueTasks.Add(task);
                }
                catch (Exception ex)
                {
                    LogWriter.LogException(ex, "There was an error while creating tasks: " + ex.Message, LoggerType.Log4Net);
                }
            }
        }

        public void Stop(CancellationToken cancellationToken)
        {
            _isRunning = false;

            try
            {
                Task.WaitAll(_processSmsQueueTasks.ToArray(), cancellationToken);
            }
            catch (Exception ex)
            {
                LogWriter.LogException(ex, "There was an error while stopping the service: " + ex.Message, LoggerType.Log4Net);
            }

            LogWriter.LogEntry("Queue processor stopped", LoggerType.Log4Net);
        }
    }
}
