// １モジュール １ファイル
module.exports = function(req, res)
{
    //--------------------------------------------------------------
    // 1. ガチャIDに一致するガチャのデータをデータストアから取得する
    //--------------------------------------------------------------

    // データストアに接続する準備
    var NCMB = require('ncmb');
    const APL_KEY = "アプリケーション キー";
    const CLI_KEY = "クライアント キー";
    var ncmb = new NCMB(APL_KEY, CLI_KEY);

    // リクエストのクエリ（ガチャID）を取得
    var gachaId = req.query.gachaId;
    if(gachaId == null){
        // ガチャIDが渡されていない
        res.status(400)
           .json({"message":"BadRequest (No gachaId)"})
    }

    // データストアの "Gacha"クラスに接続し、
    //      objectIdがgachaIdに一致するものを検索して取得（ヒットするものは１つ）
    var gachaClass = ncmb.DataStore("Gacha");
    gachaClass.equalTo("objectId", gachaId)
        .fetchAll()
        .then(function(results){
            // データストアから取得成功
            if(results.length == 0){
                // １つも見つからなかった
                res.status(404)
                    .json({"message":"NotFound (Confirm objectId)"});
            }

            //--------------------------------------------------------------
            // 2. ガチャロジック本体 (ガチャから得られる報酬３つのうち１つを決定)
            //--------------------------------------------------------------
            rewardNum = selectReward(results[0].probability);
            if(rewardNum == -1){
                res.status(500)
                    .json({"message":"Probabilities of rewards must be defined as Array(length=2)"});
            }
            // ガチャの結果
            var moneyDiff = -results[0].cost;
            var pointDiff = results[0].rewards[rewardNum];

            //--------------------------------------------------------------
            // 3. ガチャの結果が得られたらそのログを保存する
            //--------------------------------------------------------------

            // リクエストのクエリ(userId)を取得
            var userId = req.query.userId;
            if(userId == null){
                // ユーザIDが渡されていない
                res.status(400)
                   .json({"message":"BadRequest (No userId)"})
            }

            // データストアの "GachaLog"クラスに接続
            var GachaLogClass = ncmb.DataStore("GachaLog");
            var gachaLogClass = new GachaLogClass();
            // ログ保存を実行
            gachaLogClass.set("moneyDiff", moneyDiff)   // お金の増減
                         .set("pointDiff", pointDiff)   // ポイントの増減
                         .set("userId", userId)         // ユーザID
                         .save()
                         .then(function(gachaLogClass){
                             //--------------------------------------------------------------
                             // 4. JSON形式でガチャの結果とログ保存成功の旨を端末に返す
                             //--------------------------------------------------------------
                             res.status(200)
                                .json({"moneyDiff":moneyDiff,
                                       "pointDiff":pointDiff});
                         })
                         .catch(function(err){
                             // ログ保存失敗
                             res.status(500).json({"message": "Failed to save log."});
                         });
        })
        .catch(function(err){
            // データストアにエラーあり
            res.status(500).json({error: 500});
        });
}

//--------------------------------------------------------------
// ガチャの報酬３つのうちから１つを選択する関数
//--------------------------------------------------------------
function selectReward(probabilities)
{
    // probabilities は Array か
    if ( !(Array.isArray(probabilities)) ) return -1;
    // probabilities の要素数は２か
    if ( probabilities.length != 2) return -1;

    const p0 = Number(probabilities[0]); // rewards[0]が選択される確率
    const p1 = Number(probabilities[1]); // rewards[1]が   〃

    // randNum: [0.0, 1.0]
    var randNum = Math.random();
    if(randNum <= p0) return 0;
    else if(randNum <= p0+p1) return 1;
    else return 2;
}