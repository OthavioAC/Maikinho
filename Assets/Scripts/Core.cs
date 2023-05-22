using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum CorTinta // brainstorming
{
    Vermelho,
    Verde,
    Azul,
}

public static class Core
{
    // amoeda
    private static int quantidadeAmoeda = 0;

    public static int GetAmoeda()
    {
        return quantidadeAmoeda;
    }

    public static void SetAmoeda(int novaQuantidade)
    {
        quantidadeAmoeda = novaQuantidade;
        UpdateAmoeda();
    }

    public static void IncrementaAmoeda(int incremento)
    {
        quantidadeAmoeda += incremento;
        UpdateAmoeda();
    }


    // vida
    private static int pontosDeVida = 0;
    
    public static int GetPontosDeVida()
    {
        return pontosDeVida;
    }

    public static void SetPontosDeVida(int novaQuantidade)
    {
        pontosDeVida = novaQuantidade;
        UpdatePontosDeVida();
    }

    public static void IncrementaPontosDeVida(int incremento)
    {
        pontosDeVida += incremento;
        UpdatePontosDeVida();
    }

    // tinta
    private static int quantidadeTinta = 0;
    public static int GetQuantidadeTinta()
    {
        return quantidadeTinta;
    }
    public static void SetQuantidadeTinta(int novaQuantidade)
    {
        quantidadeTinta = novaQuantidade;
        UpdateNumeroTinta();
    }

    public static void IncrementaQuantidadeTinta(int incremento)
    {
        quantidadeTinta += incremento;
        UpdateNumeroTinta();
    }

    public static bool SetIndicadorGrafite(bool indicadorSetActive)
    {
        GameObject indicadorGrafite = Camera.main.transform.GetChild(0).GetChild(3).gameObject; // pessimo, mudar dps
        indicadorGrafite.SetActive(indicadorSetActive);
        return true;
    }

    private static void UpdateNumeroTinta()
    {
        Transform referenciaUI = Camera.main.transform.GetChild(0).GetChild(0);
        if(referenciaUI != null)
        {
            referenciaUI.GetComponentInChildren<TextMeshProUGUI>().text = "TINTA: " + quantidadeTinta.ToString();
        }
    }

    private static void UpdateAmoeda()
    {
        Transform referenciaUI = Camera.main.transform.GetChild(0).GetChild(2);
        if (referenciaUI != null)
        {
            referenciaUI.GetComponentInChildren<TextMeshProUGUI>().text = "AMOEDA: " + quantidadeAmoeda.ToString();
        }
    }

    private static void UpdatePontosDeVida()
    {
        Transform referenciaUI = Camera.main.transform.GetChild(0).GetChild(1);
        if (referenciaUI != null)
        {
            referenciaUI.GetComponentInChildren<TextMeshProUGUI>().text = "VIDA: " + pontosDeVida.ToString();
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
        quantidadeTinta = 0;
        quantidadeAmoeda = 0;
        pontosDeVida = 0;
    }
}
