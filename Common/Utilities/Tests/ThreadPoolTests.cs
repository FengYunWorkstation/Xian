#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ClearCanvas.Common.Utilities;
using System.Threading;

namespace ClearCanvas.Common.Utilities.Tests
{
	[TestFixture]
	public class ThreadPoolTests
	{
		private delegate void ItemDelegate<T>(T item);
		private delegate void CommandDelegate();

		private object _syncLock = new object();
		private int _enqueued;
		private int _dequeued;
		private int _expectedDequeued;

		private int _threadCount;

		private volatile bool _stopThreads;

		public ThreadPoolTests()
		{ 
		}

		private int Enqueued
		{
			get
			{
				lock (_syncLock)
				{
					return _enqueued;
				}
			}
			set
			{
				lock (_syncLock)
				{
					_enqueued = value;
				}
			}
		}

		private int Dequeued
		{
			get
			{
				lock (_syncLock)
				{
					return _dequeued;
				}
			}
			set
			{
				lock (_syncLock)
				{
					_dequeued = value;
				}
			}
		}

		private int ExpectedDequeued
		{
			get
			{
				lock (_syncLock)
				{
					return _expectedDequeued;
				}
			}
			set
			{
				lock (_syncLock)
				{
					_expectedDequeued = value;
				}
			}
		}

		private void IncrementEnqueued(int amount)
		{
			lock (_syncLock)
			{
				_enqueued += amount;
			}
		}

		private void IncrementDequeued(int amount)
		{
			lock (_syncLock)
			{
				_dequeued += amount;
			}
		}

		private void IncrementExpectedDequeued(int amount)
		{
			lock (_syncLock)
			{
				_expectedDequeued += amount;
			}
		}


		[TestFixtureSetUp]
		public void Initialize()
		{
			_stopThreads = false;
			_threadCount = 10;

			_enqueued = 0;
			_dequeued = 0;
			_expectedDequeued = 0;
		}
		
		[Test]
		public void TestBlockingQueue()
		{
			BlockingQueue<int> queue = new BlockingQueue<int>();

			ItemDelegate<int> addToQueue = delegate(int numberToAdd)
			{
				for (int i = 0; i < numberToAdd; ++i)
				{
					queue.Enqueue(1);
				}
				
				this.IncrementEnqueued(numberToAdd);
				this.IncrementExpectedDequeued(numberToAdd);
			};

			addToQueue(100000);

			ThreadStart start = delegate()
			{
				while (true)
				{
					try
					{
						queue.Dequeue();
						this.IncrementDequeued(1);
					}
					catch
					{
						if (_stopThreads)
							break; //queue is empty *and* we're supposed to quit.
					}
					
					Thread.Sleep(0);
				}
			};

			List<Thread> threads = new List<Thread>();
			for (int i = 0; i < _threadCount; ++i)
			{
				Thread thread = new Thread(start);
				thread.Start();
				threads.Add(thread);
			}

			//continually add to the queue a bit.
			int numberTimesAdded = 0;
			for (int i = 0; i < _threadCount; ++i)
			{
				addToQueue(100000);
				Thread.Sleep(5);
			}

			//'pulse' the queue by letting it go empty, then adding more.
			numberTimesAdded = 0;
			while (true)
			{
				if (queue.Count == 0)
				{
					if (++numberTimesAdded <= _threadCount)
					{
						addToQueue(100000);
					}
					else
					{
						//the real test of exiting the queue is when it's empty, not when it's non-empty.
						queue.ContinueBlocking = false;
						break;
					}
				}

				Thread.Sleep(5);
			}
			
			_stopThreads = true;

			foreach (Thread thread in threads)
				thread.Join();

			threads.Clear();

			Assert.AreEqual(_expectedDequeued, _dequeued, "expectedValue != numberDequeued");
		}

		[Test]
		public void TestSimpleThreadPool()
		{
			SimpleBlockingThreadPool pool = new SimpleBlockingThreadPool(_threadCount);
			pool.ThreadPriority = ThreadPriority.Normal;

			string failMessage = "not supposed to be able to enqueue when pool is not running and AllowInactiveAdd is false";
			try
			{
				pool.Enqueue(delegate() { ; });
			}
			catch
			{
				failMessage = null;
			}

			if (failMessage != null)
				Assert.Fail(failMessage);
			
			ItemDelegate<int> addToPool = delegate(int numberToAdd) 
			{
				for (int i = 0; i < numberToAdd; ++i)
				{
					pool.Enqueue
					(
						delegate()
						{
							this.IncrementDequeued(1);
						}
					);
				}

				this.IncrementEnqueued(numberToAdd);
				this.IncrementExpectedDequeued(numberToAdd);
			};

			pool.Start();

			TestChangeProperties(pool);

			addToPool(100000);
			pool.Stop(false);
			
			Assert.AreNotEqual(0, pool.QueueCount, "queue is empty, but it should not be because CompleteBeforeStop is currently false");

			failMessage = null;
			try
			{
				pool.AllowInactiveAdd = true;
				addToPool(100000);
			}
			catch
			{
				failMessage = "adding to the queue should be allowed because AllowInactiveAdd is true";
			}

			if (failMessage != null)
				Assert.Fail(failMessage);

			pool.Start();

			//'pulse' the queue by letting it go empty, then adding more.
			int numberTimesAdded = 0;
			while (true)
			{
				if (pool.QueueCount == 0)
				{
					addToPool(100000);
					if (++numberTimesAdded <= _threadCount)
						break;
				}

				Thread.Sleep(5);
			}

			pool.Stop(false);
			Assert.AreNotEqual(0, pool.QueueCount, "queue is empty, but it should not be because CompleteBeforeStop is currently false");

			addToPool(100000);
			pool.Start();
			pool.Stop(true);

			Assert.AreEqual(0, pool.QueueCount, "queue should be empty because CompleteBeforeStop is currently true");

			Assert.AreEqual(this.ExpectedDequeued, this.Dequeued, "expected dequeued != dequeued");
		}

		[Test]
		public void InterthreadMarshallerTest()
		{
			int someNumber = 0;
			int numberDelegatesAdded = 100000;

			InterthreadMarshaler marshaller = new InterthreadMarshaler();
			
			for (int i = 0; i < numberDelegatesAdded; ++i)
				marshaller.QueueInvoke(
					delegate() 
					{
						lock (_syncLock)
						{
							++someNumber;
						}
					});

			marshaller.Dispose();

			Assert.AreNotEqual(someNumber, numberDelegatesAdded, "the marshaller *did* finish processing all messages before being Disposed(), where it should not have.");

			marshaller = new InterthreadMarshaler();
			marshaller.Dispose();//shouldn't hang.
		}

		private void TestChangeProperties(SimpleBlockingThreadPool pool)
		{
			string failMessage = "Not supposed to be able to change 'AllowInactiveAdd' when the thread pool is active.";
			try
			{
				pool.AllowInactiveAdd = true;
			}
			catch
			{
				failMessage = null;
			}

			if (failMessage != null)
				Assert.Fail(failMessage);

			failMessage = "Not supposed to be able to change 'ThreadPriority' when the thread pool is active.";
			try
			{
				pool.ThreadPriority = ThreadPriority.Highest;
			}
			catch
			{
				failMessage = null;
			}

			if (failMessage != null)
				Assert.Fail(failMessage);

			failMessage = "Not supposed to be able to change 'Concurrency' when the thread pool is active.";
			try
			{
				pool.Concurrency = 5;
			}
			catch
			{
				failMessage = null;
			}

			if (failMessage != null)
				Assert.Fail(failMessage);
		}
	}
}

#endif
