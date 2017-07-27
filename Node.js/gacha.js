// １モジュール １ファイル
module.exports = function(req, res)
{
    //--------------------------------------------------------------
    // 1. ガチャIDに一致するガチャのデータをデータストアから取得する
    //--------------------------------------------------------------
    // ① mBaaSの機能を使う準備をする

    // ② アプリから受け取ったガチャID(gachaId)を確認

    // ③ データストアの "Gacha"クラスに接続しgachaIdに一致するものを取得
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