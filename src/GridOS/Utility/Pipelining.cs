using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        // Generic pipelining scaffolding; use case not established yet, potentially will be removed

        interface IProcessor<TIn, TOut>
        {
            TOut Process(TIn input, TOut output = default(TOut));
        }

        interface IProcessorComposite<TIn, TOut> : IProcessor<TIn, TOut>
        {
            IProcessorComposite<TIn, TOut> Add(IProcessor<TIn, TOut> processor);
            IProcessorComposite<TIn, TOut> Remove(IProcessor<TIn, TOut> processor);
        }

        interface IProcessorCache<TIn, TOut>
        {
            void RemoveFromCache(TIn key);
            void AddToCache(TIn key, TOut value);
            void Update(TIn key, TOut newValue);
            void Flush();
            IProcessor<TIn, TOut> Processor { get; }
        }

        class ProcessorCache<TIn, TOut> : IProcessor<TIn, TOut>, IProcessorCache<TIn, TOut>
        {
            protected Dictionary<TIn, TOut> _cache = new Dictionary<TIn, TOut>();
            public IProcessor<TIn, TOut> Processor => _decorated;
            protected IProcessor<TIn, TOut> _decorated;

            public TOut Process(TIn input, TOut output = default(TOut))
            {
                TOut value;
                
                // If not cached, get fresh one and cache it
                if (_cache.TryGetValue(input, out value) == false)
                {
                    value = _decorated.Process(input, output);
                    AddToCache(input, value);
                }

                output = value;
                return output;
            }

            public ProcessorCache(IProcessor<TIn, TOut> decorated)
            {
                _decorated = decorated;
            }

            public void RemoveFromCache(TIn key)
            {
                _cache.Remove(key);
            }

            public void AddToCache(TIn key, TOut value)
            {
                _cache.Add(key, value);
            }

            public void Update(TIn key, TOut newValue)
            {
                if (_cache.ContainsKey(key))
                    _cache[key] = newValue;
            }

            public void Flush()
            {
                _cache.Clear();
            }
        }

        interface ILoopSignal
        {
            bool Loop { get; }
        }

        abstract class ProcessorCompositeBase<TIn, TOut> : IProcessorComposite<TIn, TOut>
        {
            protected List<IProcessor<TIn, TOut>> _pipeline = new List<IProcessor<TIn, TOut>>();

            public abstract TOut Process(TIn input, TOut output = default(TOut));

            public virtual IProcessorComposite<TIn, TOut> Add(IProcessor<TIn, TOut> processor)
            {
                _pipeline.Add(processor);
                return this;
            }

            public virtual IProcessorComposite<TIn, TOut> Remove(IProcessor<TIn, TOut> processor)
            {
                _pipeline.Remove(processor);
                return this;
            }

            public virtual IProcessorComposite<TIn, TOut> RemoveAll()
            {
                _pipeline.Clear();
                return this;
            }
        }

        class LinearPipeline<TIn, TOut> : ProcessorCompositeBase<TIn, TOut>
        {
            public override TOut Process(TIn input, TOut output = default(TOut))
            {
                foreach (var item in _pipeline)
                {
                    output = item.Process(input, output);
                }
                return output;
            }
        }

        class LoopingPipeline<TIn, TOut> : ProcessorCompositeBase<TIn, TOut>
            where TIn : ILoopSignal
        {
            public override TOut Process(TIn input, TOut output = default(TOut))
            {
                while (true)
                {
                    foreach (var item in _pipeline)
                    {
                        output = item.Process(input, output);
                        if (!input.Loop) goto Exit;
                    }
                }
                Exit:
                return output;
            }
        }

        class LoopingTransformingPipeline<TIn, TIntermediate, TOut> : IProcessor<TIn, TOut>
            where TIn : ILoopSignal
        {
            protected List<IProcessor<TIn, TIntermediate>> _pipeline = new List<IProcessor<TIn, TIntermediate>>();
            protected IProcessor<TIntermediate, TOut> _finalProcess;

            public virtual TOut Process(TIn input, TOut output = default(TOut))
            {
                TIntermediate intermediate = default(TIntermediate);
                while (true)
                {
                    foreach (var item in _pipeline)
                    {
                        intermediate = item.Process(input, intermediate);
                        if (!input.Loop) goto Exit;
                    }
                }
                Exit:
                return _finalProcess.Process(intermediate, output);
            }

            public virtual LoopingTransformingPipeline<TIn, TIntermediate, TOut> Add(IProcessor<TIn, TIntermediate> processor)
            {
                _pipeline.Add(processor);
                return this;
            }
        }

        class LinearTransformingPipeline<TIn, TIntermediate, TOut> : IProcessor<TIn, TOut>
        {
            protected List<IProcessor<TIn, TIntermediate>> _pipeline = new List<IProcessor<TIn, TIntermediate>>();
            protected IProcessor<TIntermediate, TOut> _transformer;

            public virtual TOut Process(TIn input, TOut output = default(TOut))
            {
                TIntermediate intermediate = default(TIntermediate);
                foreach (var item in _pipeline)
                {
                    intermediate = item.Process(input, intermediate);
                }
                return _transformer.Process(intermediate, output);
            }

            public virtual LinearTransformingPipeline<TIn, TIntermediate, TOut> Add(IProcessor<TIn, TIntermediate> processor)
            {
                _pipeline.Add(processor);
                return this;
            }

            public virtual LinearTransformingPipeline<TIn, TIntermediate, TOut> Remove(IProcessor<TIn, TIntermediate> processor)
            {
                _pipeline.Remove(processor);
                return this;
            }

            public virtual LinearTransformingPipeline<TIn, TIntermediate, TOut> SetTransformer(IProcessor<TIntermediate, TOut> transformer)
            {
                _transformer = transformer;
                return this;
            }
        }
    }
}
