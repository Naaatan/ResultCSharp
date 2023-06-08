namespace Result;
public class Result<T>
{
    /// <summary>
    /// 成功/失敗の値を保持するフィールド
    /// </summary>
    internal object Value { get; private set; }

    /// <summary>
    /// このインスタンスが成功した結果を返す場合は true です
    /// </summary>
    public bool IsSuccess
    {
        get
        {
            return Value is not Failure;
        }
    }

    /// <summary>
    /// このインスタンスが失敗した結果を返す場合は true です
    /// </summary>
    public bool IsFailure
    {
        get
        {
            return Value is Failure;
        }
    }

    /// <summary>
    /// create Instance of Result
    /// </summary>
    internal Result(object value)
    {
        Value = value;
    }

    /// <summary>
    /// このインスタンスが
    /// 成功している場合は、結果のタイプ[T]を返します。 
    /// 失敗している場合は、default(T) を返します。
    /// よって、参照型のタイプ[T]であれば、null を返します。
    /// </summary>
    public T? GetOrNull()
    {
        if (IsFailure || Value == null)
        {
            return default;
        }
        else
        {
            if (Value is not T)
            {
                return default;
            }
            return (T)Value;
        }
    }

    /// <summary>
    /// このインスタンスが
    /// 成功している場合は、結果のタイプ[T]を返します。 
    /// 失敗している場合は、[defaultValue] を返します。
    /// </summary>
    public T GetOrDefault(T defaultValue)
    {
        if (IsFailure || Value == null)
        {
            return defaultValue;
        }
        else
        {
            if (Value is not T)
            {
                return defaultValue;
            }
            return (T)Value;
        }
    }

    /// <summary>
    /// このインスタンスが
    /// 失敗している場合は、例外クラスを返します。 
    /// 成功している場合は、null を返します。
    /// </summary>
    public Exception? ExceptionOrNull()
    {
        if (Value is not Failure failure)
        {
            return null;
        }
        return failure.Exception;
    }

    /// <summary>
    /// このインスタンスが
    /// 失敗している場合は、例外をスローします。
    /// 成功している場合は、スローしません。
    /// </summary>
    public void ThrowOnFailure()
    {
        var exception = ExceptionOrNull();
        if (exception != null)
        {
            throw exception;
        }
    }

    /// <summary>
    /// このインスタンスが
    /// 成功している場合は、タイプ[T]の結果を返します。
    /// 失敗している場合は、例外をスローします。
    /// </summary>
    public T GetOrThrow()
    {
        ThrowOnFailure();
        return GetOrNull() ?? throw new NullReferenceException();
    }

    /// <summary>
    /// このインスタンスが
    /// 成功している場合は、[onSuccess]を実行します。
    /// 失敗している場合は、[onSuccess] は処理されません。
    /// </summary>
    public Result<T> OnSuccess(Action<T> onSuccess)
    {
        if (IsSuccess)
        {
            onSuccess(GetOrNull());
        }
        return this;
    }

    /// <summary>
    /// このインスタンスが
    /// 失敗した場合は、[onFailure]を実行します。
    /// 成功している場合は、[onFailure] は処理されません。
    /// </summary>
    public Result<T> OnFailure(Action<Exception> onFailure)
    {
        var exception = ExceptionOrNull();
        if (exception != null)
        {
            onFailure(exception);
        }
        return this;
    }

    /// <summary>
    /// このインスタンスが
    /// 指定したタイプ[E]のExceptionで
    /// 失敗している場合は、[onFailure]を実行します。
    /// 成功している場合は、[onFailure] は処理されません。
    /// </summary>
    public Result<T> OnFailure<E>(Action<E> onFailure) where E : Exception
    {
        var exception = ExceptionOrNull();
        if (exception != null)
        {
            if (exception is E e)
            {
                onFailure(e);
            }
        }
        return this;
    }

    // -- static methods --

    /// <summary>
    /// 成功結果のResultインスタンスを生成します。 
    /// </summary>
    public static Result<R> Successful<R>(R value)
    {
        return new Result<R>(value);
    }

    /// <summary>
    /// 失敗結果のResultインスタンスを生成します。
    /// </summary>
    public static Result<R> Failed<R>(Exception exception)
    {
        return new Result<R>(new Failure(exception));
    }

    /// <summary>
    /// 任意の処理[func]を実行し、成功した場合は成功結果[R]をカプセル化して返します。
    /// 失敗した場合は発生した例外をカプセル化して返します。
    /// </summary>
    public static Result<R> RunCatching<R>(Func<R> func)
    {
        try
        {
            return Result<R>.Successful(func());
        }
        catch (Exception ex)
        {
            return Result<R>.Failed<R>(ex);
        }
    }

    /// <summary>
    /// 失敗結果を示すクラスです
    /// </summary>
    internal class Failure
    {
        public Exception Exception { get; private set; }

        public Failure(Exception exception)
        {
            Exception = exception;
        }
    }
}

