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

・更新履歴
ver1.0 メッシュのBoundsを自動調整する機能を追加。Boundsの大きさの変更や除外の設定を可能に
ver1.01 原点からずらした場所に位置するアバターの場合, Boundsが大幅にずれる不具合を修正
ver1.02 一部アバターでデフォルトのBoundsScaleだと巨大なサイズになる不具合を修正

-----------------------------------------------------
〇ProbeAnchorSetter(ver1.0)
特定のオブジェクト以下のメッシュすべてのProbeAnchorを自動設定します。
これによってメッシュごとに影の付き方が違うというのを防げます。

Unityのメニュー(FileやVRChatSDK等が並んでいるところ)にある「VRCDeveloperTool」から以下の機能を持つウィンドウが開けます

1. TargetObjectにメッシュのProbeAnchorを設定したいアバターのオブジェクトを設定してください（Animatorコンポーネントがついているオブジェクト）
2. 必要に応じて他の項目を設定して「Set ProbeAnchor」を選択すると各メッシュに自動設定されます。

・設定項目
Set To SkinnedMeshRenderer : チェックをいれるとSkinnedMeshRendererがついたメッシュを対象とします
Set To MeshRenderer : チェックをいれるとMeshRendererがついたメッシュを対象とします
TargetPosition : 光の影響の計算に使う場所を選択します
	- HEAD : Headボーン位置
	- CHEST : Chestボーン位置
	- ROOTOBJECT : TargetObjectに設定したオブジェクト位置
RendererList :	設定をおこなうメッシュの一覧です。チェックをいれると設定対象にします。
				「Select」を選択するとそのメッシュがどのオブジェクトのものか確認できます

・更新履歴
ver1.0　Renderer基準とオブジェクトごとに設定対象を選択できるように。HEAD, CHEST, ROOTOBJECTを基準として設定可能に

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

〇AnimationPropertyConvertor(ver1.0)
シェイプキーの名前の変更などによって今まで使ってたAnimationファイルのプロパティ名では使えなくなったときに
そのプロパティ名を任意のものに置き換えたAnimationファイルを作成します。

Unityのメニュー(FileやVRChatSDK等が並んでいるところ)にある「VRCDeveloperTool」から以下の機能を持つウィンドウが開けます

[使い方1 (シェイプキーのプロパティの名前を変更したい場合)]
1. 「Convert BlendShapeName」にチェックを入れます
2. 「Avatar's SkinnedMeshRenderer」で変更後のシェイプキーをもつメッシュを選択します
3. 「Pre AnimationsClips」にプロパティ名を変更したいAnimationファイルを設定していきます。
	「+」「-」で一度に設定するAnimationファイルの数を増減できます。
	設定すると、AnimationPropertyListにそのAnimationファイルに含まれているプロパティ一覧が表示されます。
	複数のAnimationファイルが設定されるときに重複するプロパティは1つだけ表示されるようになっています。
4. prePropertyを見て変更したいプロパティ名をposPropertyNameで選択します。
	その際にposPrepertyNameには2で設定したメッシュに含まれるシェイプキー一覧が表示されています。
5. AnimationPropertyListで変更したいプロパティの左側にチェックマークが入っていることを確認します。
6. AnimClipSaveFolderで変更後のAnimationファイルを保存するフォルダ―を選択します。
7. 「Convert Property & Save as New File」を選択すると新しいAnimationファイルとして変更後のファイルが作成されます。

[使い方2 (シェイプキーのプロパティ以外も含む場合)]
1. 使い方1-3をおこないます。
2. 使い方1-4をおこないます。このときにposPropertyは選択ではなく入力で新しいプロパティ名を設定します。
3. 使い方1-5～7をおこないます。

〇注意点
- 生成されるAnimationファイルは変更前と同じ名前になっています。保存先のフォルダが同じの場合、名前のあとに数字がつくようになっています

・更新履歴
ver1.0 本ツールを作成

-----------------------------------------------------

〇ShapeKeyNameChanger(ver1.1)
メッシュのシェイプキーの名前を変更します。
Meshを複製して新しいMeshを作成してシェイプキーの名前を変更しているので元データに戻すことは可能です。

Unityのメニュー(FileやVRChatSDK等が並んでいるところ)にある「VRCDeveloperTool>Mesh」から以下の機能を持つウィンドウが開けます

[使い方]
1. 「Renderer」に名前を変更したいシェイプキーを持つメッシュのオブジェクトを設定します。
（SkinnedMeshRendererコンポーネントを持つオブジェクト）
2. シェイプキーの名前を変更します。「Input」もしくは「Select」で変更方法が変わります。
	変更されるシェイプキーにはチェックが入ります。チェックを外すことで変更しないようにできます。
2-1. [Input]
	変更したいシェイプキーの右にある入力欄に変更後のシェイプキーの名前を入力します
2-2. [Select]
	変更したいシェイプキーの右にある選択欄で変更後のシェイプキーの名前を選択します
3. シェイプキーを複製して名前を変更する場合は「Duplication ShapeKeys」にチェックを入れます
4. すべての変更したいシェイプキーの入力が終わったら「Change ShapeKeyName」を選択します。
5. 4の後にシェイプキーの変更をなかったことにしたい場合は変更後にCtrl+Zで変更前のシェイプキーの名称に戻ります。

VRCDeveloperToolフォルダ内の
ShapeKeyNameChanger/Resources/ShapeKeyNameChanger/shapekeynames.jsonを
変更すればSelectで出てくる選択肢を変更できます

〇注意点
- シェイプキーの名称が変更された後のメッシュはfbxがあるフォルダと同じフォルダに追加されます。
- fbxそのものが書き換えられるわけではないので変更した内容はUnity上でのみ有効です
(Blender等の別ツールに変更したアバターのfbxファイルをインポートしても変更は反映されていません)

・更新履歴
ver 1.1 シェイプキー名を選択して変更する機能とシェイプキー名を複製して名前を変更する機能を追加
ver1.0.1 VRCDeveloperTool/Meshの下に表示されるように変更
ver1.0 本ツールを作成

-----------------------------------------------------

〇ShapeKeyMixer(ver1.0)
メッシュの複数のシェイプキーを合成します
Meshを複製して新しいMeshを作成してシェイプキーを合成しているので元データに戻すことは可能です。

Unityのメニュー(FileやVRChatSDK等が並んでいるところ)にある「VRCDeveloperTool>Mesh」から以下の機能を持つウィンドウが開けます

[使い方]
1. 「Renderer」に合成したいシェイプキーを持つメッシュのオブジェクトを設定します。
（SkinnedMeshRendererコンポーネントを持つオブジェクト）
2. 合成したいシェイプキーにチェックをいれます。
3. 合成元のシェイプキーを残しておく場合は「Delete Origin ShapeKey」のチェックを外してください。
4. 合成後のシェイプキーの名前を「Mixed ShapeKey Name」に入力してください。
5. 合成したいシェイプキーを全て選択したら「Mix ShapeKeys」を選択します。
6. 5の後にシェイプキーの合成をなかったことにしたい場合は変更後にCtrl+Zで変更前のシェイプキーの状態に戻ります。

〇注意点
- シェイプキーが結合された後のメッシュはfbxがあるフォルダと同じフォルダに追加されます。
- fbxそのものが書き換えられるわけではないので変更した内容はUnity上でのみ有効です
(Blender等の別ツールに変更したアバターのfbxファイルをインポートしても変更は反映されていません)

・更新履歴
ver1.0 本ツールを作成

-----------------------------------------------------
〇SubMeshDeleter(ver1.0.1)
特定のサブメッシュを削除します
Meshを複製して新しいMeshを作成してサブメッシュを削除しているので元データに戻すことは可能です。

Unityのメニュー(FileやVRChatSDK等が並んでいるところ)にある「VRCDeveloperTool>Mesh」から以下の機能を持つウィンドウが開けます

[使い方]
1. 「SkinnedMeshRenderer」に削除したいサブメッシュを持つメッシュのオブジェクトを設定します。
（SkinnedMeshRendererコンポーネントを持つオブジェクト）
2. 削除したいサブメッシュにチェックをいれます。
3. 必要があれば「SelectFolder」を押してメッシュの保存先を変更します
4. 削除したいサブメッシュをすべて選択したら「Delete SubMesh」を選択します
5. 4の後にサブメッシュの削除をなかったことにしたい場合は変更後にCtrl+Zで変更前のメッシュの状態に戻ります。

〇注意点
- サブメッシュが削除されたあとのメッシュはデフォルトであればfbxと同じフォルダに追加されます。
- fbxそのものが書き換えられるわけではないので変更した内容はUnity上でのみ有効です
(Blender等の別ツールに変更したアバターのfbxファイルをインポートしても変更は反映されていません)

・更新履歴
ver1.0.1 メッシュの法線と接線の扱いを変更
ver1.0 本ツールを作成

-----------------------------------------------------

〇ShapeKeyReorder(ver1.0)
メッシュのシェイプキーの順番を変更します。
Meshを複製して新しいMeshを作成してシェイプキーの名前を変更しているので元データに戻すことは可能です。

Unityのメニュー(FileやVRChatSDK等が並んでいるところ)にある「VRCDeveloperTool>Mesh」から以下の機能を持つウィンドウが開けます

[使い方]
1. 「Renderer」に名前を変更したいシェイプキーを持つメッシュのオブジェクトを設定します。
（SkinnedMeshRendererコンポーネントを持つオブジェクト）
2. マウスを使ってリストの要素を並び替えます。
または「AutoSort」の各ボタンで自動的に順番を並び替えます。
 - UnSort : 未ソート状態にします
 - VRChat Default : VRChat推奨の順番にします（特定の4つのシェイプキーを一番最初に持ってきています）
 - A-Z : シェイプキー名による昇順ソートです。
 - Z-A ; シェイプキー名による降順ソートです。
3. 順番が決まったら「Change ShapeKey order」を選択します。
4. 3の後にシェイプキーの変更をなかったことにしたい場合は変更後にCtrl+Zで変更前のシェイプキーの順番に戻ります。

〇注意点
- fbxそのものが書き換えられるわけではないので変更した内容はUnity上でのみ有効です
(Blender等の別ツールに変更したアバターのfbxファイルをインポートしても変更は反映されていません)

・更新履歴
ver1.0 本ツールを作成

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

●開発・動作確認環境
Unity 2018.4.11f1

-----------------------------------------------------
ご意見, ご要望があればTwitter: @gatosyocoraまで