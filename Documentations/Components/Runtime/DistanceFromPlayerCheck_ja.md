﻿# DistanceFromPlayerCheck

#### **Namespace**: Unity.TinyCharacterController
---

## 概要:
`DistanceFromPlayerCheck` は、"Player"タグを持つオブジェクトからの距離を取得し、指定された範囲内にプレイヤーが入ったときにコールバックを発行するコンポーネントです。このコンポーネントを持つオブジェクトのリストを収集し、"Player"タグを持つオブジェクトとの距離を計算して`Distance`に格納します。距離インデックスは`_ranges`に基づいており、距離インデックスが変更された場合、`OnChangeDistanceIndex`が呼び出されます。コンポーネントが無効の場合、計算は行われず、`DistanceIndex`は-1、`Distance`は`float.MaxValue`に設定されます。

## 機能と操作:
- **プレイヤーからの距離範囲**: プレイヤーからの距離を範囲で指定します。例えば1, 5, 8と指定された場合、プレイヤーからの距離が2.3mなら1、7mなら2となります。
- **距離範囲変更時のコールバック**: プレイヤーからの距離の範囲が変更されたときに呼び出されるコールバックです。

## プロパティ
| 名前 | 説明 |
|------------------|------|
| `_ranges` | プレイヤーからの距離の範囲を指定する配列です。 |
| `DistanceIndex` | 現在のプレイヤーからの距離（範囲）インデックスです。コンポーネントが登録されていないか、プレイヤーが存在しない場合は-1を返します。 |
| `Distance` | プレイヤーからの距離です。コンポーネントが登録されていないか、プレイヤーが存在しない場合は`float.MaxValue`を返します。 |
| `OnChangeDistanceIndex` | 距離インデックスが変更されたときに発生するイベントです。 |

## メソッド
| 名前 | 機能 |
|------------------|------|
| ``public static List<GameObject>`` GetIndexObjects( ``int distanceIndex`` ) | "指定された距離インデックスに対応するオブジェクトのリストを取得します。" |
