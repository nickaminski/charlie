﻿using charlie.dto.Card;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charlie.dal.interfaces
{
    public interface ICardRepository
    {
        Task<IEnumerable<Card>> GetAllCardsInSet(string setName);
        Task WriteCardDataToSetFile(string data, string setName);
        Task<Card> GetById(int id);
        Task<Card> GetByName(string name);
        Task WriteCardDataToAllFile(string data);
        Task<CardCollection> GetCollection();
        Task AddToCollection(IEnumerable<Card> newCards);
        void DeleteCollection();
    }
}
