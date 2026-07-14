# Concurrency model (GeoXplorer)

This document is the **new-code default** for async / networking work in Unity
scripts under `Assets/Scripts/`. Existing coroutines are left alone unless a
ticket rewrites them.

## Defaults for new code

| Prefer | Avoid |
|---|---|
| `async Task` methods | `async void` (except Unity event handlers) |
| `CancellationToken` on long loops | `while (true)` with no cancel path |
| Shared / static `HttpClient` per type | `new HttpClient()` per call |
| `IEnumerator` only when a Unity API requires it (`WaitForEndOfFrame`, yield instructions) | Starting unbounded background threads |

**UniTask:** Recommended for future adoption (better Unity player-loop integration),
but **not** a project dependency yet. Leave that call to the project lead.

## `async void`

Allowed **only** for Unity event-handler signatures where the engine requires
`void` (e.g. `async void OnPointerClick(...)`). Every `async void` body must
wrap work in `try/catch` and log failures.

`Start` / `Awake` / `OnEnable` must **not** be `async void`. Pattern:

```csharp
void Start()
{
    _ = RunStartAsync();
}

async Task RunStartAsync()
{
    try
    {
        await InitializeAsync();
    }
    catch (Exception ex)
    {
        Debug.LogException(ex);
    }
}
```

## Long-running loops

Any loop that polls a network or waits indefinitely must accept a
`CancellationToken` and exit on cancel:

```csharp
async Task PollAsync(CancellationToken token)
{
    while (!token.IsCancellationRequested)
    {
        await FetchOnceAsync(token);
        await Task.Delay(500, token);
    }
}
```

Callers own a `CancellationTokenSource` and cancel it from `OnDestroy` /
`StopWatching` / scene teardown.

## HttpClient

Create **one** `HttpClient` (static field or injected singleton) per logical
service type. Do not dispose it per request.

```csharp
public class AnchorExchanger
{
    private static readonly HttpClient SharedHttpClient = new HttpClient();

    public async Task<string> RetrieveLastAnchorKey()
    {
        return await SharedHttpClient.GetStringAsync(baseAddress + "/last");
    }
}
```

## When `IEnumerator` is fine

Keep or write coroutines when you must:

- `yield return null` / `WaitForEndOfFrame` / `WaitForSeconds`
- Drive Unity APIs that only expose coroutine entry points
- Bridge from existing `StartCoroutine` call sites

Do **not** migrate all existing coroutines in one pass; convert at rewrite
boundaries (networking, anchors, menus).

## Residual / justified sites (post #28)

| Site | Notes |
|---|---|
| `AnchorExchanger.WatchKeys` | Uses `CancellationToken` + shared `HttpClient`; cancel via `StopWatching()` |
| Existing `IEnumerator` in menus / lobbies | Left alone; convention applies to new code |

## Grep checks

```bash
rg -n 'async void' Assets/Scripts --glob '*.cs'
# expect: none, or only event handlers with try/catch

rg -n 'Task\.Factory\.StartNew|Task\.Run\(|new Thread' Assets/Scripts --glob '*.cs'
# expect: none without CancellationToken documentation

rg -n 'new HttpClient' Assets/Scripts --glob '*.cs'
# expect: none in hot paths (shared static field initializer is OK)
