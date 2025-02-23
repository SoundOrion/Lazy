`System.Lazy<T>` は、C# などの .NET において「遅延初期化 (Lazy Initialization)」をサポートするクラスです。オブジェクトの作成を遅らせ、実際に必要になった時に初期化することで、パフォーマンスの最適化やリソースの節約を行うことができます。

---

## **基本的な使い方**
```csharp
using System;

class Program
{
    static void Main()
    {
        // Lazy<T> を使用して遅延初期化
        Lazy<HeavyObject> lazyObject = new Lazy<HeavyObject>();

        Console.WriteLine("オブジェクトはまだ作成されていません。");

        // 初めて .Value にアクセスするとオブジェクトが生成される
        HeavyObject obj = lazyObject.Value;

        Console.WriteLine("オブジェクトが作成されました。");
    }
}

class HeavyObject
{
    public HeavyObject()
    {
        Console.WriteLine("HeavyObject のコンストラクタが呼ばれました！");
    }
}
```

### **出力**
```
オブジェクトはまだ作成されていません。
HeavyObject のコンストラクタが呼ばれました！
オブジェクトが作成されました。
```
上記のコードでは、`Lazy<HeavyObject>` を使用することで `HeavyObject` のインスタンスが `lazyObject.Value` にアクセスするまで作成されません。

---

## **コンストラクタの指定**
### **1. デフォルトコンストラクタを使用する場合**
```csharp
Lazy<HeavyObject> lazyObject = new Lazy<HeavyObject>();
```
この場合、`HeavyObject` のデフォルトコンストラクタが使用されます。

### **2. ファクトリメソッドを指定する場合**
```csharp
Lazy<HeavyObject> lazyObject = new Lazy<HeavyObject>(() => new HeavyObject("カスタム値"));
```
この方法では、特定の引数を持つコンストラクタや別のインスタンス作成ロジックを指定できます。

---

## **スレッドセーフな遅延初期化**
`Lazy<T>` は、デフォルトでスレッドセーフ (`LazyThreadSafetyMode.ExecutionAndPublication`) になっていますが、明示的にスレッドセーフのモードを設定することもできます。

### **スレッドセーフのモード**
```csharp
Lazy<HeavyObject> lazyObject = new Lazy<HeavyObject>(() => new HeavyObject(), 
    System.Threading.LazyThreadSafetyMode.PublicationOnly);
```
- `ExecutionAndPublication` (デフォルト)  
  → 複数のスレッドが同時にアクセスしても、オブジェクトが1回だけ初期化される
- `PublicationOnly`  
  → 競合する複数のスレッドが異なるインスタンスを作成する可能性がある
- `None`  
  → スレッドセーフではないが、高速

---

## **利点**
✅ **リソースの節約**  
  - 必要になったときだけオブジェクトを作成するため、メモリやCPUリソースの節約になる

✅ **パフォーマンス向上**  
  - アプリケーションの起動時に重いオブジェクトの初期化を遅らせることで、起動時間を短縮できる

✅ **スレッドセーフ**  
  - `Lazy<T>` を使えば、マルチスレッド環境でも安全に遅延初期化を行える

---

## **まとめ**
- `System.Lazy<T>` は、オブジェクトの作成を遅延させるためのクラス
- `lazyObject.Value` にアクセスしたときに初期化が行われる
- スレッドセーフな遅延初期化を簡単に実装できる

ゲーム開発や大規模なシステム開発で、重い処理の初期化を遅らせる場合に便利な機能です！いい指摘ですね！ 確かに `Lazy<T>` を使わずに「必要になったときに直接インスタンスを作成する」という方法もあります。その場合、`Lazy<T>` を使う利点は何なのかを考えてみましょう。

---

### **1. `Lazy<T>` を使わずに遅延初期化する方法**
`Lazy<T>` を使わずに、単純に `null` チェックをして初期化する方法もあります。

```csharp
class Program
{
    static HeavyObject? _instance;

    static HeavyObject GetHeavyObject()
    {
        if (_instance == null)
        {
            _instance = new HeavyObject();
        }
        return _instance;
    }

    static void Main()
    {
        Console.WriteLine("オブジェクトはまだ作成されていません。");

        HeavyObject obj = GetHeavyObject(); // ここで初めて作成される

        Console.WriteLine("オブジェクトが作成されました。");
    }
}
```

この方法でも、`Lazy<T>` を使わずに必要な時にインスタンスを作ることができます。

---

### **2. `Lazy<T>` を使うメリット**
それでも **`Lazy<T>` を使う理由** はいくつかあります。

#### **✅ 1. スレッドセーフな実装が簡単**
上記の `GetHeavyObject` メソッドは、**マルチスレッド環境では競合が発生する** 可能性があります。例えば、2つのスレッドが同時に `GetHeavyObject()` を呼び出した場合、`_instance` がまだ `null` なら、両方のスレッドが `new HeavyObject()` を実行してしまうことがあります。

**対策**として、手動で `lock` を使うとこうなります：

```csharp
private static readonly object _lock = new object();
static HeavyObject? _instance;

static HeavyObject GetHeavyObject()
{
    if (_instance == null)
    {
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = new HeavyObject();
            }
        }
    }
    return _instance;
}
```

手動で `lock` を使うのはコードが長くなり、ミスが起こりやすくなります。一方で、`Lazy<T>` はデフォルトでスレッドセーフなので、**一行で済むし、安全** です。

```csharp
Lazy<HeavyObject> _lazyInstance = new Lazy<HeavyObject>();

HeavyObject obj = _lazyInstance.Value; // 自動でスレッドセーフに
```

---

#### **✅ 2. `Lazy<T>` は明示的で、意図が伝わりやすい**
`Lazy<T>` を使うと、「これは遅延初期化したいオブジェクトである」という意図が明確になります。

たとえば、チーム開発では、手動で `null` チェックするコードよりも `Lazy<T>` を使うほうが意図が伝わりやすく、コードの可読性が向上します。

---

#### **✅ 3. 遅延評価とファクトリメソッドの組み合わせ**
`Lazy<T>` は、コンストラクタではなく**遅延評価のロジックを自由に指定** できる点もメリットです。

```csharp
Lazy<HeavyObject> _lazyInstance = new Lazy<HeavyObject>(() =>
{
    Console.WriteLine("遅延評価でオブジェクトを作成！");
    return new HeavyObject();
});
```
このように、単なる `new` ではなく、ファクトリメソッドや**条件分岐を含む初期化処理** を遅延させることができます。

---

### **結論**
`Lazy<T>` を使わなくても「必要な時に初期化する」ことは可能ですが、

- **スレッドセーフな遅延初期化** を簡単に実装したい場合
- **可読性を向上させたい場合**
- **複雑な初期化ロジックをカプセル化したい場合**

には `Lazy<T>` を使うのが便利です。  
一方で、**シンプルなケースでは手動で遅延初期化するのもアリ** なので、ケースバイケースで選択するのが良いですね！いい質問ですね！  
C# のマルチスレッドは **`Task.Run()` だけではなく、いくつかの方法** があります。  
`Task.Run()` はマルチスレッドを使う一つの手段ですが、それ以外にも `Thread` や `Parallel` などの方法もあります。

---

## **C# でのマルチスレッドの主な手法**
1. **`Task.Run()` (Task ベースの並行処理)**
2. **`Thread` クラス (低レベルスレッド管理)**
3. **`Parallel` クラス (並列処理)**
4. **`async/await` (非同期処理)**
5. **`ThreadPool` (軽量スレッド管理)**

---

## **1. `Task.Run()`（最も一般的なマルチスレッド手法）**
`Task.Run()` は、**バックグラウンドスレッドで非同期に処理を実行** するときによく使います。

### **🌟 基本的な例**
```csharp
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Mainスレッド開始");

        Task task = Task.Run(() =>
        {
            Console.WriteLine($"Task実行: スレッドID {Environment.CurrentManagedThreadId}");
            Task.Delay(2000).Wait(); // 擬似的に時間がかかる処理
            Console.WriteLine("Task完了");
        });

        await task; // メインスレッドをブロックせず待機
        Console.WriteLine("Mainスレッド終了");
    }
}
```
**💡 特徴**
- **スレッドプールからスレッドを取得して実行する**（新しいスレッドは作らない）
- **`async/await` と組み合わせて非同期処理を実装できる**
- **複数の `Task` を同時に実行できる**
- **スレッドプールを使うので、`Thread` クラスより軽量**

---

## **2. `Thread` クラス（古典的な方法）**
低レベルで直接スレッドを管理したい場合は `Thread` クラスを使います。

### **🌟 基本的な例**
```csharp
using System;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.WriteLine("Mainスレッド開始");

        Thread thread = new Thread(() =>
        {
            Console.WriteLine($"新しいスレッド実行: スレッドID {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(2000); // 擬似的に時間がかかる処理
            Console.WriteLine("スレッド完了");
        });

        thread.Start();
        thread.Join(); // スレッドが終了するまで待つ

        Console.WriteLine("Mainスレッド終了");
    }
}
```
**💡 特徴**
- **新しいスレッドを作成して実行する**（スレッドプールを使わない）
- **`Join()` でスレッドの終了を待つことができる**
- **手動で管理する必要があるので、`Task` よりも扱いが難しい**

**🆚 `Task.Run()` との違い**
- `Thread` は**手動でスレッドを作成**する（オーバーヘッドが大きい）
- `Task.Run()` は**スレッドプールを活用**して管理が自動化されている

今では **`Task.Run()` を使うのが一般的** で、`Thread` クラスを使うことは少ないです。

---

## **3. `Parallel` クラス（データ並列処理向け）**
複数のデータに対して並列で処理を行いたいときに便利なのが `Parallel` クラスです。

### **🌟 `Parallel.ForEach()` の例**
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        var numbers = new List<int> { 1, 2, 3, 4, 5 };

        Parallel.ForEach(numbers, number =>
        {
            Console.WriteLine($"処理中: {number}, スレッドID {Environment.CurrentManagedThreadId}");
        });

        Console.WriteLine("並列処理完了");
    }
}
```
**💡 特徴**
- **`Parallel.ForEach()` はリストの要素を並列で処理する**
- **`Parallel.For()` でループ処理も並列化できる**
- **データが大量にある場合に特に有効**

---

## **4. `async/await`（非同期処理だが、マルチスレッドと関係あり）**
`async/await` は **非同期処理のための構文** ですが、内部的に `Task.Run()` やスレッドプールを使って並行処理を行うことがあります。

### **🌟 `async/await` の例**
```csharp
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Mainスレッド開始");

        await DoWorkAsync();

        Console.WriteLine("Mainスレッド終了");
    }

    static async Task DoWorkAsync()
    {
        await Task.Delay(2000); // 2秒待つ（非同期）
        Console.WriteLine("非同期処理完了");
    }
}
```
**💡 特徴**
- **マルチスレッドではなく、シングルスレッド上で非同期に処理を切り替える**
- **非同期I/O（ファイル・ネットワーク通信）に最適**
- **CPU バウンドな処理（計算など）には向かない（その場合は `Task.Run()` を使う）**

---

## **5. `ThreadPool`（軽量スレッド管理）**
スレッドプール (`ThreadPool`) は、事前にスレッドを作成し、繰り返し使うことでパフォーマンスを最適化します。

### **🌟 `ThreadPool` の例**
```csharp
using System;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.WriteLine("Mainスレッド開始");

        ThreadPool.QueueUserWorkItem(_ =>
        {
            Console.WriteLine($"スレッドプールで実行: スレッドID {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(2000);
            Console.WriteLine("スレッドプールの処理完了");
        });

        Thread.Sleep(3000); // メインスレッドが終了しないように待機
        Console.WriteLine("Mainスレッド終了");
    }
}
```
**💡 特徴**
- **スレッドプールを使って、スレッドの生成・破棄を最適化**
- **`Task.Run()` の内部でもスレッドプールが使われている**
- **直接 `ThreadPool` を使うことは少なく、`Task.Run()` で代用されることが多い**

---

## **結論**
### **「`Task.Run()` を使うときがマルチスレッド？」**
👉 確かに `Task.Run()` は **マルチスレッドの代表的な使い方の1つ** ですが、  
👉 **他にも `Thread`、`Parallel`、`ThreadPool` などの方法がある！**  

`Task.Run()` は **「スレッドを明示的に作成せずに、並行処理を実行したい」** 場合に最適です。  
一方で、`Thread` クラスは **「明示的にスレッドを管理したい」** 場合に使います（ただし、今ではほぼ `Task` に置き換えられています）。

### **使い分け**
✅ **CPU負荷が高い処理 → `Task.Run()`**  
✅ **データ並列処理 → `Parallel.ForEach()`**  
✅ **非同期I/O（ネットワークやDB）→ `async/await`**  
✅ **手動スレッド管理 → `Thread`（ただし推奨されない）**  
✅ **スレッドプールを使う → `ThreadPool`（通常は `Task` でOK）**  

要するに、**`Task.Run()` が一番使いやすく、`Thread` は今ではあまり使われない** ってことですね！とても鋭い視点ですね！  
確かに `Thread` クラスを直接使うことはほぼなくなり、`Task.Run()` や `async/await` が主流になっています。  
そのうえで「**`Lazy<T>` と `Task.Run()` を併用するケースが見えない**」という疑問はもっともです。  

---

## **Lazy<T> が役に立つ場面は？**
マルチスレッド環境での `Lazy<T>` の有用性は **「どのスレッドが `Lazy<T>` を使うか」** によります。  
大きく分けて次のようなパターンがあります。

### **パターン① マルチスレッド環境で「1回だけ」初期化する必要がある**
`Lazy<T>` は **複数のスレッドからアクセスされたときに、安全に1回だけ初期化する** のが得意です。

🔹 **例：設定の読み込み（アプリ全体で1回だけ）**
```csharp
Lazy<AppConfig> _config = new Lazy<AppConfig>(() => LoadConfig(), true);

static AppConfig LoadConfig()
{
    Console.WriteLine("設定をロード中...");
    Task.Delay(2000).Wait(); // 擬似的な遅延
    return new AppConfig("MyApp", "1.0");
}

static void Main()
{
    Parallel.Invoke(
        () => Console.WriteLine($"スレッド1: {_config.Value.AppName}"),
        () => Console.WriteLine($"スレッド2: {_config.Value.AppName}")
    );
}
```
**☑ ポイント**
- `Parallel.Invoke()` を使い、複数のスレッドから `_config.Value` にアクセス
- **どのスレッドが最初に `_config.Value` を取得しても、安全に1回だけ `LoadConfig()` が実行される**

**💡 `Lazy<T>` がない場合は `lock` で制御する必要がある**
```csharp
private static AppConfig? _config;
private static readonly object _lock = new object();

static AppConfig GetConfig()
{
    if (_config == null)
    {
        lock (_lock)
        {
            if (_config == null)
            {
                _config = LoadConfig();
            }
        }
    }
    return _config;
}
```
👉 **この `lock` の手間を `Lazy<T>` が省略してくれる**

---

### **パターン② 遅延初期化をしたいが、スレッドプール（Task.Run）と組み合わせる必要がある**
`Lazy<T>` は **「値の作成が時間がかかるが、必要になるまで遅らせたい」** という場合にも使えます。  
しかし、もしその処理が**重い計算**なら、`Task.Run()` でバックグラウンド処理にした方がいい場合があります。

🔹 **例：データベース接続を遅延初期化**
```csharp
Lazy<Task<DatabaseConnection>> _lazyDbConnection = new Lazy<Task<DatabaseConnection>>(() =>
    Task.Run(() => InitializeDatabaseConnection()));

static async Task Main()
{
    Console.WriteLine("データベースに接続を開始");

    DatabaseConnection db = await _lazyDbConnection.Value; // 初回だけTask.Run()で非同期に実行
    Console.WriteLine("データベース接続完了");

    // 以降のアクセスは即座に取得
    DatabaseConnection db2 = await _lazyDbConnection.Value;
}
```
**☑ ポイント**
- `_lazyDbConnection.Value` は **最初の1回だけ `Task.Run()` で実行される**
- **次回以降はすぐに `Task<DatabaseConnection>` を返せる**
- **シングルスレッドでも `await` で非同期に初期化できる**

---

### **パターン③ 非同期の遅延初期化 (`Lazy<T>` ではなく `AsyncLazy<T>` を作る)**
実は `Lazy<T>` 自体は `async/await` をサポートしていません。  
つまり、**`Lazy<Task<T>>` でラップする方法** が一般的です。

🔹 **AsyncLazy クラスを作る**
```csharp
public class AsyncLazy<T>
{
    private readonly Lazy<Task<T>> _lazy;

    public AsyncLazy(Func<Task<T>> factory)
    {
        _lazy = new Lazy<Task<T>>(() => Task.Run(factory));
    }

    public Task<T> Value => _lazy.Value;
}
```
🔹 **使い方**
```csharp
AsyncLazy<string> _lazyString = new AsyncLazy<string>(async () =>
{
    await Task.Delay(2000);
    return "遅延初期化された値";
});

static async Task Main()
{
    Console.WriteLine("値の取得開始...");
    string value = await _lazyString.Value;
    Console.WriteLine($"取得した値: {value}");
}
```
**☑ ポイント**
- `AsyncLazy<T>` で **非同期の遅延初期化が可能**
- **`Task.Run()` を使ってバックグラウンドスレッドで実行**
- **`await` で簡単に使える**

---

## **結論**
「`Lazy<T>` は `Task.Run()` とどう組み合わせるの？」という疑問に対する答えは **「ケースによる」** です。

### **✅ `Lazy<T>` が役立つケース**
1. **マルチスレッド環境で安全に1回だけ初期化したい**
2. **重い初期化処理を遅らせたい**
3. **スレッドセーフに値をキャッシュしたい**

### **✅ `Task.Run()` と組み合わせるべきケース**
1. **初期化が重く、非同期で実行したい（`Lazy<Task<T>>` を使う）**
2. **初期化がCPUバウンド（計算処理）であり、バックグラウンドスレッドで実行したい**
3. **`async/await` に統一したい（`AsyncLazy<T>` を使う）**

---

### **💡 最後に**
- **`Lazy<T>` は「シンプルな遅延初期化」**
- **`Task.Run()` は「非同期実行」**
- **最近のC# では `AsyncLazy<T>` を使うことが増えている**
- **ただし、シンプルな `Lazy<T>` は今でも有用！**

要するに、「**スレッドセーフな遅延初期化は今でも意味があるが、非同期時代には少し工夫が必要**」ということですね！
そうです！ **`Lazy<T>` の初期化処理自体は同期的** に実行されます。  
`Lazy<Task<T>>` を使う場合、`Lazy<T>` の部分は同期的に処理されますが、  
その `T` が `Task<T>` なので、実際の初期化ロジック（`Task.Run(factory)` の部分）は非同期に実行されます。

---

## **`Lazy<Task<T>>` の動作の流れ**
以下のコードで、`AsyncLazy<string>` の動作を詳しく見ていきます。

```csharp
public class AsyncLazy<T>
{
    private readonly Lazy<Task<T>> _lazy;

    public AsyncLazy(Func<Task<T>> factory)
    {
        _lazy = new Lazy<Task<T>>(() => Task.Run(factory));
    }

    public Task<T> Value => _lazy.Value;
}
```
### **1. `AsyncLazy<T>` の初期化**
```csharp
AsyncLazy<string> _lazyString = new AsyncLazy<string>(async () =>
{
    Console.WriteLine("値の遅延初期化開始");
    await Task.Delay(2000);
    Console.WriteLine("値の遅延初期化完了");
    return "遅延初期化された値";
});
```
この `AsyncLazy<T>` を作成する時点では、  
**まだ値の生成（`factory` の呼び出し）は行われていません。**

**つまり、`Lazy<Task<T>>` の `Lazy<T>` 部分は「待機状態」** であり、  
`Value` プロパティにアクセスするまでは `Task.Run()` が実行されません。

---

### **2. `await _lazyString.Value` にアクセス**
```csharp
static async Task Main()
{
    Console.WriteLine("値の取得開始...");
    
    string value = await _lazyString.Value;

    Console.WriteLine($"取得した値: {value}");
}
```
ここで `Value` にアクセスした瞬間に、`Lazy<Task<T>>` の `Lazy<T>` が  
初期化されて `Task.Run(factory)` が実行されます。

---

## **同期と非同期の動作の違い**
`Lazy<T>` 自体は **同期的** に `Value` を取得するだけですが、  
その `Value` が `Task<T>` であるため、  
**実際の初期化処理 (`factory()`) は非同期的に実行される** という仕組みです。

### **具体的な流れ**
| ステップ | 処理 | 同期 or 非同期 |
|---------|------|-------------|
| 1. `AsyncLazy<T>` のコンストラクタ実行 | `Lazy<Task<T>>` の初期化 | **同期** |
| 2. `Value` にアクセス | `Lazy<T>` の `Value` が初期化される | **同期** |
| 3. `Task.Run(factory)` 実行 | `factory()` の非同期処理が開始 | **非同期** |
| 4. `await` で待機 | `factory()` が終わるまで待機 | **非同期** |

---

## **なぜ `Lazy<Task<T>>` ではなく `AsyncLazy<T>` を作るのか？**
`Lazy<Task<T>>` をそのまま使うこともできますが、  
この場合、`Task<T>` を直接作成するので `Task.Run()` で明示的にバックグラウンドスレッドを使えません。

例えば、次のように `Lazy<Task<T>>` を使うと、  
**初期化時にすぐ `Task<T>` が作られるため、スレッドの管理がやや不透明になります。**

```csharp
Lazy<Task<string>> _lazyTask = new Lazy<Task<string>>(() => LoadDataAsync());

async Task<string> LoadDataAsync()
{
    await Task.Delay(2000);
    return "データ";
}
```
`AsyncLazy<T>` の場合は **明示的に `Task.Run()` を使って**、  
バックグラウンドスレッドで処理を実行できるので、  
「遅延評価 & 非同期初期化」が **より明確に制御可能** になります。

---

## **結論**
✅ **`Lazy<T>` の初期化処理は同期**  
✅ **`Lazy<T>` の `Value` は同期的に取得されるが、その `T` が `Task<T>` なら非同期処理が可能**  
✅ **`AsyncLazy<T>` では `Task.Run()` を使うことで、非同期の遅延初期化がしやすくなる**  

つまり **「Lazy は同期的に動作するが、その中で非同期処理を実行させることは可能」** ということですね！


自作の `AsyncLazy<T>` をもっと進化させるなら、以下のポイントを考えると良さそうです。

---

## **🚀 `AsyncLazy<T>` の改善ポイント**
1. **`await asyncLazy` の直感的な使い方をサポート**
   - `await asyncLazy.Value` の代わりに `await asyncLazy` できるようにする
   - `GetAwaiter()` を実装する

2. **キャンセル (`CancellationToken`) 対応**
   - 初期化中にキャンセルできるようにする
   - `Task.Run()` に `CancellationToken` を渡せるようにする

3. **リトライ機能**
   - 失敗した場合に **再試行 (retry)** できるようにする
   - `Task` の例外をキャッチしてリトライできるようにする

4. **非同期のロック (`SemaphoreSlim`) を使って初期化の競合を防ぐ**
   - `Lazy<T>` はスレッドセーフだが、`Task.Run()` の実行を安全にするために `SemaphoreSlim` を利用する

---

## **✨ 進化版 `AsyncLazy<T>` の実装**
```csharp
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

public class AsyncLazy<T>
{
    private readonly Func<CancellationToken, Task<T>> _factory;
    private readonly Lazy<Task<T>> _lazy;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public AsyncLazy(Func<Task<T>> factory)
        : this((_) => factory()) { }

    public AsyncLazy(Func<CancellationToken, Task<T>> factory)
    {
        _factory = factory;
        _lazy = new Lazy<Task<T>>(() => Task.Run(() => ExecuteAsync(CancellationToken.None)));
    }

    private async Task<T> ExecuteAsync(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            return await _factory(cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Task<T> Value => _lazy.Value;

    // 直接 `await asyncLazy` を使えるようにする
    public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();

    // キャンセル機能をサポートした `Value`
    public Task<T> GetValueAsync(CancellationToken cancellationToken = default)
    {
        return _lazy.Value.ContinueWith(task => task.Result, cancellationToken);
    }
}
```

---

## **✨ 進化版 `AsyncLazy<T>` の使い方**
### **🌟 基本的な使い方**
```csharp
AsyncLazy<string> _lazyString = new AsyncLazy<string>(async () =>
{
    await Task.Delay(2000);
    return "遅延初期化された値";
});

static async Task Main()
{
    Console.WriteLine("値の取得開始...");

    // ✅ `await asyncLazy` でシンプルに使える
    string value = await _lazyString;
    
    Console.WriteLine($"取得した値: {value}");
}
```

---

### **🌟 キャンセル機能を活用**
```csharp
CancellationTokenSource cts = new CancellationTokenSource();

AsyncLazy<string> _lazyWithCancel = new AsyncLazy<string>(async (cancellationToken) =>
{
    await Task.Delay(5000, cancellationToken); // 途中でキャンセルされる可能性あり
    return "キャンセル可能な値";
});

static async Task Main()
{
    CancellationTokenSource cts = new CancellationTokenSource();

    var task = _lazyWithCancel.GetValueAsync(cts.Token);

    // 途中でキャンセル
    cts.CancelAfter(2000);

    try
    {
        string value = await task;
        Console.WriteLine($"取得した値: {value}");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("値の取得がキャンセルされました");
    }
}
```

---

## **✨ 進化版 `AsyncLazy<T>` のポイント**
✅ **`await asyncLazy` でシンプルに書ける (`GetAwaiter()` を実装)**  
✅ **キャンセル (`CancellationToken`) をサポート**  
✅ **非同期ロック (`SemaphoreSlim`) で競合防止**  
✅ **`Task.Run()` でバックグラウンドスレッドで実行**  

---

## **🚀 さらなる進化アイデア**
- **リトライ機能**
  - `ExecuteAsync()` で失敗時の再試行をサポート
- **エラーハンドリング**
  - `Lazy<Task<T>>` はエラーをキャッシュするので、例外が発生した場合のリセット機能を追加
- **Time-to-Live (TTL)**
  - キャッシュの有効期限を持たせ、一定時間後に再生成

---

💡 **これなら汎用的に使える `AsyncLazy<T>` が完成！** 🚀