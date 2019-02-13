本セットに含まれる各種Editor拡張に関しての説明と利用規約を記載しています。

----------------------------------------------------
〇ComponentAdder (ver1.21)
指定したオブジェクトの直接的な子オブジェクトすべてに特定のコンポーネントをつけます(Current_Child_Only)
ver1.1からすべての子オブジェクトにもつけられるモード(All_Children)を追加しました

ParentObjectより下のの子オブジェクトに対してコンポーネントの追加・変更・削除がおこなわれます
チェックボックスにチェックを入れたコンポーネントが対象となり、パラメータを設定可能なものはチェック後にパラメータが表示されます

Unityのメニュー(FileやVRChatSDK等が並んでいるところ)にある「VRCDeveloperTool」から以下の機能を持つウィンドウが開けます

・更新履歴
ver1.0 Rigidbody, VRC_Pickup, VRC_ObjectSyncに対応
ver1.1 RigidbodyでFreeze系を操作可能に, コンポーネントの一括削除可能に, 子オブジェクト以降すべてに追加可能に
ver1.2 BoxColliderに対応(既にBoxCollider以外のColliderがついている場合、追加されない)
ver1.21 All_Childrenの内部処理を簡略化

-----------------------------------------------------
〇HandPoseAdder(ver1.2)
指定したAnimationファイルに特定の手の形にするAnimationキーを追加します。
Fist, HandOpen, HandGun, FingerPoint, ThumbsUp, RocknRoll, Victoryの手の形に設定可能。
ver 1.1より自分で用意した手の形も設定可能にしました(CustomHandPose)

1. 追加したいAnimationファイルのInspectorの名前付近で右クリックをすることで表示される「Add Hand pose '**'」(**はポーズ名)を選択すると
Animationファイルの0と1フレーム目に選択した手の形にするAnimationキーが追加されます。
「Clear Hand pose」を選択すると手の形が未設定状態のAnimationファイルになります。

CustomHandPoseはUnityのメニューにある「VRCDeveloperTool」の「Hand Pose Adder Setting」を選択すると設定ウィンドウが開きます。
「Hand Pose Name」と「Hand Pose AnimationClip」に任意の手の形を設定し「Apply Setting」を選択するとFist等と同様の方法で追加できます。
「Load Setting」を選択するとCustomHandPoseに設定中の情報が「Hand Pose Name」と「Hand Pose AnimationClip」に表示されます。
「Clear Setting」を選択するとCustomHandPoseに何も設定されていない状態になります。
設定する手の形のAnimationファイルはHandPoseAdder/Animationsにあるものを参考にしてください。

Animationファイルはgatosyocoraが作成しました。取扱いに関しては利用規約をご覧ください。
また、CustomHandPoseによって自動生成されたスクリプトに関しても利用規約が適用されます。

・更新履歴
ver1.0 設定可能な手の形にFist, HandOpen, HandGun, FingerPoint, ThumbsUp, RocknRoll, Victoryを追加
ver1.1 自分で用意した手の形を設定可能に
ver1.1.1 FingerPointとRocknRollのAnimationファイルの一部値が0,1フレームで異なっていたのを修正
ver1.2 手の形を未設定状態にする機能を追加

-----------------------------------------------------
〇HumanoidPoseResetter(ver1.1)
アニメーションオーバーライドの設定等で変更されたHumanoidオブジェクトのポーズを元に戻します。

[使い方1]
Unityのメニュー(FileやVRChatSDK等が並んでいるところ)にある「VRCDeveloperTool」から以下の機能を持つウィンドウが開けます
1. TargetObjectにポーズをリセットしたいアバターのオブジェクトを設定してください（Animatorコンポーネントがついているオブジェクト）
2. 「Reset Pose」を選択するとポーズがリセットされます
Ctrl+Zでポーズを変更前に戻すこともできます

[使い方2]
1. UnityのHierarchyでポーズをリセットしたいアバターのオブジェクトを右クリックします（Animatorコンポーネントがついているオブジェクト）
2. VRCDeveloperTool>Reset Pose　を選択するとポーズがリセットされます

・更新履歴
ver1.0 Humanoidオブジェクトのポーズをリセットする機能を追加
ver1.1 Hierarchyでオブジェクトを右クリックでもリセットできるようになりました

-----------------------------------------------------
〇MeshBoundsSetter(ver1.02)
特定のオブジェクト以下のメッシュのBoundsをオブジェクト全体を囲うように自動調整します。

Unityのメニュー(FileやVRChatSDK等が並んでいるところ)にある「VRCDeveloperTool」から以下の機能を持つウィンドウが開けます

1. TargetObjectにメッシュのBoundsを調整したいアバターのオブジェクトを設定してください（Animatorコンポーネントがついているオブジェクト）
2. 「Set Bounds」を選択するとBoundsがいい感じに調整されます
デフォルトだとBoundsが(1, 2, 1)のサイズになりますが、「Bounds Scale」を変更することでこのサイズを変更できます
「Exclusions」に設定したオブジェクトのメッシュのBoundsは変更されないようにできます

・既知のバグ
アバターによってはデフォルトのBoundsだと巨大なサイズになる
→BoundsScaleを調整してください

・更新履歴
ver1.0 メッシュのBoundsを自動調整する機能を追加。Boundsの大きさの変更や除外の設定を可能に
ver1.01 原点からずらした場所に位置するアバターの場合, Boundsが大幅にずれる不具合を修正
ver1.02 一部アバターでデフォルトのBoundsScaleだと巨大なサイズになる不具合を修正

-----------------------------------------------------
〇ScaleLimitVisualizer(ver1.0)
範囲の大きさを視覚化します。

〇使い方
1. ScaleLimitVisualizerフォルダ内のScaleLimitVisualizer.prefabをHierarchyもしくはSceneにD&Dします
2-1. 必要に応じてScaleLimitVisualizerコンポーネントのScaleLimitとIsWireFrameを変更してください
2-2. ScaleLimitVisualizerオブジェクトの位置を変えると枠の位置も変わります
3. ワールドとしてアップロードしたり、UnityPackageにする際にはScaleLimitVisualizerオブジェクトは含めないでください(削除推奨)

・更新履歴
ver1.0　大きさの変更と非ワイヤーフレーム機能を追加

-----------------------------------------------------

●利用規約
本規約は本商品に含まれるすべてのスクリプトやファイルに共通で適用されるものとする。
本商品を使用したことによって生じた問題に関してはgatosyocoraは一切の責任を負わない。

・スクリプト
本スクリプトはzlibライセンスで運用される。
著作権はgatosyocoraに帰属する。

・Animationファイル
また、同封されているAnimationファイルはパラメータの一部を含め、商用利用・改変・二次配布を許可する。
その際には作者名や配布元等は記載しなくてもよい。
しかし、本Animationファイルの使用や配布により生じた問題等に関しては作者は一切の責任を負わない。

-----------------------------------------------------
ご意見, ご要望があればTwitter: @gatosyocoraまで