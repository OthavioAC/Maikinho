using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum COR_TINTA // brainstorming
{
    Vermelho,
    Verde,
    Azul,
}

public static class Core
{
    // tinta
    private static int quantidadeTinta = 0;
    public static int GetQuantidadeTinta()
    {
        return quantidadeTinta;
    }
    public static void IncrementaQuantidadeTinta(int incremento)
    {
        quantidadeTinta += incremento;
        UpdateNumeroTinta();
    }
    public static void SetQuantidadeTinta(int novaQuantidade)
    {
        quantidadeTinta = novaQuantidade;
        UpdateNumeroTinta();
    }

    private static void UpdateNumeroTinta()
    {
        Transform referenciaUI = Camera.main.transform.GetChild(0);
        if(referenciaUI != null)
        {
            referenciaUI.GetComponentInChildren<TextMeshProUGUI>().text = quantidadeTinta.ToString();
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
}
