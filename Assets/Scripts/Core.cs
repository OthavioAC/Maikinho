using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum CorTinta
{
    VERMELHA,
    AMARELA,
    VERDE,
    CIANO,
    AZUL,
    MAGENTA,
    TODAS,
}

public enum GameOverState
{
    BAD_ENDING,
    GOOD_ENDING,
    NONE,
}

public static class Core
{
    private static GameOverState gameOverState = GameOverState.NONE;
    // propriedades
    private static float gravidade = 4f; // "constante"
    private static int quantidadeMoeda = 0;
    private static int pontosDeVida = 0;
    private static int[] quantidadeTinta = { 0, 0, 0, 0, 0, 0 };
    //
    public static GameOverState GetGameOverState()
    {
        return gameOverState;
    }
    // gravidade
    public static float GetGravidade()
    {
        return gravidade;
    }
    public static void SetGravidade(float novaGravidade)
    {
        gravidade = novaGravidade;
    }
    // moedas
    public static int GetQuantidadeMoeda()
    {
        return quantidadeMoeda;
    }
    public static void SetQuantidadeMoeda(int novaQuantidade)
    {
        quantidadeMoeda = novaQuantidade;
        if (quantidadeMoeda < 0) quantidadeMoeda = 0; // cap
        UpdateDisplayEconomia();
    }
    public static void IncrementaQuantidadeMoeda(int incremento)
    {
        quantidadeMoeda += incremento;
        if (quantidadeMoeda < 0) quantidadeMoeda = 0; // cap
        UpdateDisplayEconomia();
    }
    // vida
    public static int GetPontosDeVida()
    {
        return pontosDeVida;
    }
    public static void SetPontosDeVida(int novaQuantidade)
    {
        pontosDeVida = novaQuantidade;
        if (pontosDeVida < 0) pontosDeVida = 0; // cap
        UpdateDisplayVida();
    }
    public static void IncrementaPontosDeVida(int incremento)
    {
        pontosDeVida += incremento;
        if (pontosDeVida < 0) pontosDeVida = 0; // cap
        UpdateDisplayVida();
    }
    // tinta
    public static int GetQuantidadeTinta(CorTinta corSelecionada)
    {
        return quantidadeTinta[(int)corSelecionada];
    }
    public static void SetQuantidadeTinta(CorTinta corSelecionada, int novaQuantidade)
    {
        for (int index = 0; index < (int)CorTinta.TODAS; index++)
        {
            if (corSelecionada != CorTinta.TODAS && index != (int)corSelecionada) continue;
            quantidadeTinta[index] = novaQuantidade >= 0 ? novaQuantidade : 0;
        }
        UpdateDisplayTinta(corSelecionada);
    }
    public static void IncrementaQuantidadeTinta(CorTinta corSelecionada, int incremento)
    {
        for (int index = 0; index < (int)CorTinta.TODAS; index++)
        {
            if (corSelecionada != CorTinta.TODAS && index != (int)corSelecionada) continue;
            quantidadeTinta[index] += incremento;
            if (quantidadeTinta[index] < 0) quantidadeTinta[index] = 0;
        }
        UpdateDisplayTinta(corSelecionada);
    }
    // ui
    public static bool SetIndicadorGrafite(bool indicadorSetActive, int[] custo)
    {
        GameObject indicadorGrafite = Camera.main.transform.GetChild(0).GetChild(3).gameObject; // pessimo, mudar dps
        indicadorGrafite.SetActive(indicadorSetActive);
        indicadorGrafite.GetComponentInChildren<TextMeshProUGUI>().text =
            "Custo" + 
            (custo[0] > 0 ? "\nR:" + custo[0] : string.Empty) +
            (custo[1] > 0 ? "\nY:" + custo[1] : string.Empty) +
            (custo[2] > 0 ? "\nG:" + custo[2] : string.Empty) +
            (custo[3] > 0 ? "\nC:" + custo[3] : string.Empty) +
            (custo[4] > 0 ? "\nB:" + custo[4] : string.Empty) +
            (custo[5] > 0 ? "\nM:" + custo[5] : string.Empty);
        return true;
    }
    public static bool SetIndicadorBarzin(bool indicadorSetActive)
    {
        GameObject indicadorGrafite = Camera.main.transform.GetChild(0).GetChild(4).gameObject; // pessimo, mudar dps
        indicadorGrafite.SetActive(indicadorSetActive);
        return true;
    }
    private static void UpdateDisplayTinta(CorTinta corSelecionada)
    {
        Transform referenciaUI = Camera.main.transform.GetChild(0).GetChild(0);
        if (corSelecionada == CorTinta.TODAS)
        {
            foreach (Sprite spritePart in Resources.LoadAll<Sprite>("tintas"))
            {
                if (spritePart.name.StartsWith("TINTA_VERMELHA_0"))
                {
                    referenciaUI.GetChild(5).GetComponent<Image>().sprite = spritePart;
                    referenciaUI.GetChild(5).GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
                }
                if (spritePart.name.StartsWith("TINTA_AMARELA_0"))
                {
                    referenciaUI.GetChild(4).GetComponent<Image>().sprite = spritePart;
                    referenciaUI.GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
                }
                if (spritePart.name.StartsWith("TINTA_VERDE_0"))
                {
                    referenciaUI.GetChild(3).GetComponent<Image>().sprite = spritePart;
                    referenciaUI.GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
                }
                if (spritePart.name.StartsWith("TINTA_CIANO_0"))
                {
                    referenciaUI.GetChild(2).GetComponent<Image>().sprite = spritePart;
                    referenciaUI.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
                }
                if (spritePart.name.StartsWith("TINTA_AZUL_0"))
                {
                    referenciaUI.GetChild(1).GetComponent<Image>().sprite = spritePart;
                    referenciaUI.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
                }
                if (spritePart.name.StartsWith("TINTA_MAGENTA_0"))
                {
                    referenciaUI.GetChild(0).GetComponent<Image>().sprite = spritePart;
                    referenciaUI.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
                }
            }
            return;
        }
        Transform referenciaTintaUI = referenciaUI.GetChild(5 - (int)corSelecionada); // ph
        if (referenciaUI != null)
        {
            int quantidadeAtual = GetQuantidadeTinta(corSelecionada);
            referenciaTintaUI.GetComponentInChildren<TextMeshProUGUI>().text = quantidadeAtual <= 1 ? string.Empty : ((quantidadeAtual < 10 ? "x" : string.Empty) + quantidadeAtual.ToString());
            string spriteName = "TINTA_" + corSelecionada.ToString() + (quantidadeAtual > 0 ? "_1" : "_0");
            foreach (Sprite spritePart in Resources.LoadAll<Sprite>("tintas"))
            {
                if(spritePart.name == spriteName)
                {
                    referenciaTintaUI.GetComponent<Image>().sprite = spritePart;
                }
            }
        }
    }
    private static void UpdateDisplayEconomia()
    {
        Transform referenciaUI = Camera.main.transform.GetChild(0).GetChild(2);
        if (referenciaUI != null)
        {
            referenciaUI.GetComponentInChildren<TextMeshProUGUI>().text = "x" + quantidadeMoeda.ToString();
        }
    }
    private static void UpdateDisplayVida()
    {
        Transform referenciaUI = Camera.main.transform.GetChild(0).GetChild(1);
        if (referenciaUI != null)
        {
            Sprite[] vida = Resources.LoadAll<Sprite>("vida");
            switch (pontosDeVida)
            {
                default:
                    /* CORACAO EXTRA */
                    referenciaUI.GetChild(3).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(4).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(5).GetComponent<Image>().sprite = vida[3];
                    /* CORACAO NORMAL */
                    referenciaUI.GetChild(0).GetComponent<Image>().sprite = vida[0];
                    referenciaUI.GetChild(1).GetComponent<Image>().sprite = vida[0];
                    referenciaUI.GetChild(2).GetComponent<Image>().sprite = vida[0];
                    break;
                case 1:
                    /* CORACAO EXTRA */
                    referenciaUI.GetChild(3).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(4).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(5).GetComponent<Image>().sprite = vida[3];
                    /* CORACAO NORMAL */
                    referenciaUI.GetChild(0).GetComponent<Image>().sprite = vida[0];
                    referenciaUI.GetChild(1).GetComponent<Image>().sprite = vida[0];
                    referenciaUI.GetChild(2).GetComponent<Image>().sprite = vida[1];
                    break;
                case 2:
                    /* CORACAO EXTRA */
                    referenciaUI.GetChild(3).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(4).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(5).GetComponent<Image>().sprite = vida[3];
                    /* CORACAO NORMAL */
                    referenciaUI.GetChild(0).GetComponent<Image>().sprite = vida[0];
                    referenciaUI.GetChild(1).GetComponent<Image>().sprite = vida[0];
                    referenciaUI.GetChild(2).GetComponent<Image>().sprite = vida[2];
                    break;
                case 3:
                    /* CORACAO EXTRA */
                    referenciaUI.GetChild(3).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(4).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(5).GetComponent<Image>().sprite = vida[3];
                    /* CORACAO NORMAL */
                    referenciaUI.GetChild(0).GetComponent<Image>().sprite = vida[0];
                    referenciaUI.GetChild(1).GetComponent<Image>().sprite = vida[1];
                    referenciaUI.GetChild(2).GetComponent<Image>().sprite = vida[2];
                    break;
                case 4:
                    /* CORACAO EXTRA */
                    referenciaUI.GetChild(3).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(4).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(5).GetComponent<Image>().sprite = vida[3];
                    /* CORACAO NORMAL */
                    referenciaUI.GetChild(0).GetComponent<Image>().sprite = vida[0];
                    referenciaUI.GetChild(1).GetComponent<Image>().sprite = vida[2];
                    referenciaUI.GetChild(2).GetComponent<Image>().sprite = vida[2];
                    break;
                case 5:
                    /* CORACAO EXTRA */
                    referenciaUI.GetChild(3).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(4).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(5).GetComponent<Image>().sprite = vida[3];
                    /* CORACAO NORMAL */
                    referenciaUI.GetChild(0).GetComponent<Image>().sprite = vida[1];
                    referenciaUI.GetChild(1).GetComponent<Image>().sprite = vida[2];
                    referenciaUI.GetChild(2).GetComponent<Image>().sprite = vida[2];
                    break;
                case 6:
                    /* CORACAO EXTRA */
                    referenciaUI.GetChild(3).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(4).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(5).GetComponent<Image>().sprite = vida[3];
                    /* CORACAO NORMAL */
                    referenciaUI.GetChild(0).GetComponent<Image>().sprite = vida[2];
                    referenciaUI.GetChild(1).GetComponent<Image>().sprite = vida[2];
                    referenciaUI.GetChild(2).GetComponent<Image>().sprite = vida[2];
                    break;
                case 7:
                    /* CORACAO EXTRA */
                    referenciaUI.GetChild(3).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(4).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(5).GetComponent<Image>().sprite = vida[4];
                    /* CORACAO NORMAL */
                    referenciaUI.GetChild(0).GetComponent<Image>().sprite = vida[2];
                    referenciaUI.GetChild(1).GetComponent<Image>().sprite = vida[2];
                    referenciaUI.GetChild(2).GetComponent<Image>().sprite = vida[2];
                    break;
                case 8:
                    /* CORACAO EXTRA */
                    referenciaUI.GetChild(3).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(4).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(5).GetComponent<Image>().sprite = vida[5];
                    /* CORACAO NORMAL */
                    referenciaUI.GetChild(0).GetComponent<Image>().sprite = vida[2];
                    referenciaUI.GetChild(1).GetComponent<Image>().sprite = vida[2];
                    referenciaUI.GetChild(2).GetComponent<Image>().sprite = vida[2];
                    break;
                case 9:
                    /* CORACAO EXTRA */
                    referenciaUI.GetChild(3).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(4).GetComponent<Image>().sprite = vida[4];
                    referenciaUI.GetChild(5).GetComponent<Image>().sprite = vida[5];
                    /* CORACAO NORMAL */
                    referenciaUI.GetChild(0).GetComponent<Image>().sprite = vida[2];
                    referenciaUI.GetChild(1).GetComponent<Image>().sprite = vida[2];
                    referenciaUI.GetChild(2).GetComponent<Image>().sprite = vida[2];
                    break;
                case 10:
                    /* CORACAO EXTRA */
                    referenciaUI.GetChild(3).GetComponent<Image>().sprite = vida[3];
                    referenciaUI.GetChild(4).GetComponent<Image>().sprite = vida[5];
                    referenciaUI.GetChild(5).GetComponent<Image>().sprite = vida[5];
                    /* CORACAO NORMAL */
                    referenciaUI.GetChild(0).GetComponent<Image>().sprite = vida[2];
                    referenciaUI.GetChild(1).GetComponent<Image>().sprite = vida[2];
                    referenciaUI.GetChild(2).GetComponent<Image>().sprite = vida[2];
                    break;
                case 11:
                    /* CORACAO EXTRA */
                    referenciaUI.GetChild(3).GetComponent<Image>().sprite = vida[4];
                    referenciaUI.GetChild(4).GetComponent<Image>().sprite = vida[5];
                    referenciaUI.GetChild(5).GetComponent<Image>().sprite = vida[5];
                    /* CORACAO NORMAL */
                    referenciaUI.GetChild(0).GetComponent<Image>().sprite = vida[2];
                    referenciaUI.GetChild(1).GetComponent<Image>().sprite = vida[2];
                    referenciaUI.GetChild(2).GetComponent<Image>().sprite = vida[2];
                    break;
                case 12:
                    /* CORACAO EXTRA */
                    referenciaUI.GetChild(3).GetComponent<Image>().sprite = vida[5];
                    referenciaUI.GetChild(4).GetComponent<Image>().sprite = vida[5];
                    referenciaUI.GetChild(5).GetComponent<Image>().sprite = vida[5];
                    /* CORACAO NORMAL */
                    referenciaUI.GetChild(0).GetComponent<Image>().sprite = vida[2];
                    referenciaUI.GetChild(1).GetComponent<Image>().sprite = vida[2];
                    referenciaUI.GetChild(2).GetComponent<Image>().sprite = vida[2];
                    break;

            }
        }
        if (pontosDeVida <= 0)
        {
            gameOverState = GameOverState.BAD_ENDING;
            SceneManager.LoadScene("GameOver"); // placeholder, colocar delay depois
        }
    }

    /*
    private static string UltimaRuaVisitada = "MainMenu";
    private static string RuaAtual = "MainMenu";

    public static void MudarRua(string NomeRua)
    {
        UltimaRuaVisitada = RuaAtual;
        RuaAtual = NomeRua;
        SceneManager.LoadScene(NomeRua);
    }
    */

    public static void Reset()
    {
        quantidadeTinta[(int)CorTinta.VERMELHA] = 0;
        quantidadeTinta[(int)CorTinta.AMARELA] = 0;
        quantidadeTinta[(int)CorTinta.VERDE] = 0;
        quantidadeTinta[(int)CorTinta.CIANO] = 0;
        quantidadeTinta[(int)CorTinta.AZUL] = 0;
        quantidadeTinta[(int)CorTinta.MAGENTA] = 0;
        quantidadeMoeda = 0;
        pontosDeVida = 0;
    }
    // utilidades
    public static float AceleracaoDeDepieri(float movimentoHorizontalAtual, float direcaoHorizontal, float aceleracao, float multiplicadorAceleracao)
    {
        return movimentoHorizontalAtual + ((movimentoHorizontalAtual < direcaoHorizontal * multiplicadorAceleracao) ? aceleracao : ((movimentoHorizontalAtual > direcaoHorizontal * multiplicadorAceleracao) ? -aceleracao : -(direcaoHorizontal / 10f))) + (direcaoHorizontal / 10f);
    }
}
