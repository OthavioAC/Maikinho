using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum CorTinta
{
    Vermelho,
    Amarelo,
    Verde,
    Ciano,
    Azul,
    Magenta,
}

public static class Core
{
    // propriedades
    private static float gravidade = 4f; // "constante"
    private static int quantidadeMoeda = 0;
    private static int pontosDeVida = 0;
    private static int[] quantidadeTinta = { 0, 0, 0, 0, 0, 0 };
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
        UpdateDisplayEconomia();
    }
    public static void IncrementaQuantidadeMoeda(int incremento)
    {
        quantidadeMoeda += incremento;
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
        UpdateDisplayVida();
    }
    public static void IncrementaPontosDeVida(int incremento)
    {
        pontosDeVida += incremento;
        UpdateDisplayVida();
    }
    // tinta
    public static int GetQuantidadeTinta(CorTinta corSelecionada)
    {
        return quantidadeTinta[(int)corSelecionada];
    }
    public static void SetQuantidadeTinta(CorTinta corSelecionada, int novaQuantidade)
    {
        quantidadeTinta[(int)corSelecionada] = novaQuantidade;
        UpdateDisplayTinta();
    }
    public static void IncrementaQuantidadeTinta(CorTinta corSelecionada, int incremento)
    {
        quantidadeTinta[(int)corSelecionada] += incremento;
        UpdateDisplayTinta();
    }
    // ui
    public static bool SetIndicadorGrafite(bool indicadorSetActive)
    {
        GameObject indicadorGrafite = Camera.main.transform.GetChild(0).GetChild(3).gameObject; // pessimo, mudar dps
        indicadorGrafite.SetActive(indicadorSetActive);
        return true;
    }
    private static void UpdateDisplayTinta()
    {
        Transform referenciaUI = Camera.main.transform.GetChild(0).GetChild(0);
        if(referenciaUI != null)
        {
            referenciaUI.GetComponentInChildren<TextMeshProUGUI>().text =
                "[R:" + quantidadeTinta[(int)CorTinta.Vermelho].ToString() +
                "|Y:" + quantidadeTinta[(int)CorTinta.Amarelo].ToString() +
                "|G:" + quantidadeTinta[(int)CorTinta.Verde].ToString() +
                "|C:" + quantidadeTinta[(int)CorTinta.Ciano].ToString() +
                "|B:" + quantidadeTinta[(int)CorTinta.Azul].ToString() +
                "|M:" + quantidadeTinta[(int)CorTinta.Magenta].ToString() + "]";
        }
    }
    private static void UpdateDisplayEconomia()
    {
        Transform referenciaUI = Camera.main.transform.GetChild(0).GetChild(2);
        if (referenciaUI != null)
        {
            referenciaUI.GetComponentInChildren<TextMeshProUGUI>().text = "MOEDA: " + quantidadeMoeda.ToString();
        }
    }
    private static void UpdateDisplayVida()
    {
        Transform referenciaUI = Camera.main.transform.GetChild(0).GetChild(1);
        if (referenciaUI != null)
        {
            string buffer = ""; // placeholder
            switch (pontosDeVida)
            {
                case 1:
                    buffer = "|3";
                    break;
                case 2:
                    buffer = "<3";
                    break;
                case 3:
                    buffer = "|3<3";
                    break;
                case 4:
                    buffer = "<3<3";
                    break;
                case 5:
                    buffer = "|3<3<3";
                    break;
                case 6:
                    buffer = "<3<3<3";
                    break;
            }
            referenciaUI.GetComponentInChildren<TextMeshProUGUI>().text = buffer;
        }
    }
    // ruas
    /* NAO ESTA SENDO USADA AINDA (só no "menu") */
    private static string UltimaRuaVisitada = "MainMenu";
    private static string RuaAtual = "MainMenu";

    public static void MudarRua(string NomeRua)
    {
        UltimaRuaVisitada = RuaAtual;
        RuaAtual = NomeRua;
        SceneManager.LoadScene(NomeRua);
    }

    public static void Reset()
    {
        quantidadeTinta[(int)CorTinta.Vermelho] = 0;
        quantidadeTinta[(int)CorTinta.Amarelo] = 0;
        quantidadeTinta[(int)CorTinta.Verde] = 0;
        quantidadeTinta[(int)CorTinta.Ciano] = 0;
        quantidadeTinta[(int)CorTinta.Azul] = 0;
        quantidadeTinta[(int)CorTinta.Magenta] = 0;
        quantidadeMoeda = 0;
        pontosDeVida = 0;
    }
    // utilidades
    public static float AceleracaoDeDepieri(float movimentoHorizontalAtual, float direcaoHorizontal, float aceleracao, float multiplicadorAceleracao)
    {
        return movimentoHorizontalAtual + ((movimentoHorizontalAtual < direcaoHorizontal * multiplicadorAceleracao) ? aceleracao : ((movimentoHorizontalAtual > direcaoHorizontal * multiplicadorAceleracao) ? -aceleracao : -(direcaoHorizontal / 10f))) + (direcaoHorizontal / 10f);
    }
}
